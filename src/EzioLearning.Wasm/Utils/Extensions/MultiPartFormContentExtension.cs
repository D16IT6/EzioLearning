using System.Collections;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace EzioLearning.Wasm.Utils.Extensions
{
    public static class MultiPartFormContentExtension
    {
        public static MultipartFormDataContent CreateFormContent(this MultipartFormDataContent content, object model, string prefix = "", string[]? nameOfFileContent = null, string[]? excludeProperties = null)
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(prefix) && prefix.EndsWith("]")) prefix += ".";
                var properties = model.GetType().GetProperties();

                properties = properties.Where(x => excludeProperties == null || !excludeProperties.Contains(x.Name))
                    .ToArray();

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
                            content.Add(new StringContent(dateValue.Value.ToString("yyyy-MM-dd")),
                                prefix + property.Name);
                        }
                        else
                        {
                            var valueString = value.ToString();
                            if (string.IsNullOrWhiteSpace(valueString)) continue;

                            content.Add(new StringContent(valueString), prefix + property.Name);
                        }
                    }
                    else if (property.PropertyType.GetInterfaces().Any(x =>
                                 x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        var enumerable = (IList)value;

                        if (enumerable.Count <= 0) continue;

                        var tempType = enumerable[0]!.GetType();
                        if (tempType.IsPrimitive || tempType == typeof(Guid) || tempType == typeof(string) ||
                            tempType.IsEnum)
                        {
                            foreach (var item in enumerable)
                            {
                                var itemString = item.ToString();
                                if (!string.IsNullOrWhiteSpace(itemString))
                                {
                                    content.Add(new StringContent(itemString), prefix + property.Name);
                                }
                            }
                        }
                        else if (tempType.IsClass)
                        {
                            for (var i = 0; i < enumerable.Count; i++)
                            {
                                var item = enumerable[i];
                                if (item != null && nameOfFileContent is { Length: >= 1 })
                                {
                                    content.CreateFormContent(item, $"{prefix}{property.Name}[{i}]",
                                        nameOfFileContent: nameOfFileContent[1..]);
                                }
                            }
                        }
                    }

                    else if (nameOfFileContent != null &&
                             typeof(IBrowserFile).IsAssignableFrom(property.PropertyType) &&
                             !string.IsNullOrWhiteSpace(nameOfFileContent[0]))
                    {
                        var file = (IBrowserFile?)value;
                        if (file == null || file.Size <= 0) break;
                        var fileContent = new StreamContent(file.OpenReadStream(file.Size));
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        content.Add(fileContent, prefix + nameOfFileContent[0], file.Name);
                    }
                }

                return content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
