using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzioLearning.Core.Dtos.Auth
{
	public class ForgotPasswordDto
	{

		public string? Email { get; set; }
		public string? ClientConfirmUrl { get; set; }
	}
}
