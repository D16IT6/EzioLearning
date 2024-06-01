using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace EzioLearning.Wasm.Utils.Extensions
{
    public static class MultiPartFormContentExtension
    {
        public static MultipartFormDataContent CreateFormContent(this MultipartFormDataContent content, object model, string? nameOfFileContent = null)
        {
            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                if (value == null) continue;

                if (property.PropertyType == typeof(string) ||
                    property.PropertyType.IsPrimitive ||
                    property.PropertyType.IsValueType)
                {
                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        var dateValue = (DateTime?)value;
                        content.Add(new StringContent(dateValue.Value.ToString("yyyy-MM-dd")), property.Name);
                    }
                    else
                    {
                        var valueString = value.ToString();
                        if (string.IsNullOrWhiteSpace(valueString)) continue;

                        content.Add(new StringContent(valueString), property.Name);
                    }
                }
                else if (typeof(IEnumerable<>).IsAssignableFrom(property.PropertyType))
                {
                    var enumerable = (IEnumerable<Guid>)value;
                    foreach (var item in enumerable)
                    {
                        content.Add(new StringContent(item.ToString()), property.Name);
                    }
                }
                else if (typeof(IBrowserFile).IsAssignableFrom(property.PropertyType) && !string.IsNullOrWhiteSpace(nameOfFileContent))
                {
                    var file = (IBrowserFile)value;
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, nameOfFileContent, file.Name);
                }
            }

            return content;
        }
    }
}
