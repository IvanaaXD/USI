using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleLangLang.ConsoleApp.DTO
{
    public class PropertyValidator<T> where T : new()
    {
        private readonly T _dtoInstance;

        public PropertyValidator(T dtoInstance)
        {
            _dtoInstance = dtoInstance ?? throw new ArgumentNullException(nameof(dtoInstance));
        }

        public bool IsValid()
        {
            MethodInfo isValidMethod = typeof(T).GetMethod("IsValid");

            if (isValidMethod == null)
            {
                throw new ArgumentException($"Method 'IsValid' not found in type {typeof(T).Name}");
            }

            return (bool)isValidMethod.Invoke(_dtoInstance, null);
        }
        public string ValidateProperty(string propertyName)
        {
            MethodInfo validatePropertyMethod = typeof(T).GetMethod("ValidateProperty");

            if (validatePropertyMethod == null)
            {
                throw new ArgumentException($"Method 'ValidateProperty' not found in type {typeof(T).Name}");
            }

            return (string)validatePropertyMethod.Invoke(_dtoInstance, new object[] { propertyName });
        }
    }
}
