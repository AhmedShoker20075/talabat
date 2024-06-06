using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
	public class RegisterDto
	{
        [Required]
        [EmailAddress]
        public string Email { get; set; }


		[Required]
		public string DisplayName { get; set; }


		[Required]
		[Phone]
		public string PhoneNumber { get; set; }


		[Required]
		[RegularExpression("(?=^.{6,10}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%&amp;*()_+]).*$",
			ErrorMessage ="Password Must Be Contain 1 UpperCase & 1 Lowercase & 1 Digit & 1 Specialcharacter !")]
		public string Password { get; set; }
    }
}
