using System.ComponentModel.DataAnnotations;

namespace CustomAttribute.Utility
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {

        private readonly string[] _extendfile={""};
        
        
        public AllowedExtensionsAttribute(string[] extendfile,string ErrorMessage="Dinh dang file khong dung") : base(ErrorMessage)
        {
            _extendfile=extendfile;

        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file= value as IFormFile;
            var extension=Path.GetExtension(file?.FileName);
            if(file!=null)
            {
                if(!_extendfile.Contains(extension))
                {

                    return new ValidationResult(this.ErrorMessage);

                }

            }
            return ValidationResult.Success;
        }


    }
}