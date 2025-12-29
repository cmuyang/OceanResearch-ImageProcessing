using OceanReseach.Client;

namespace OceanResearch.Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        public static string AuthToken;
        public static string CurrentUser; 

        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
        }
    }
}

/*
 * -		$exception	{"Unable to load one or more of the requested types.\r\nCould not load type 'Microsoft.OpenApi.Any.IOpenApiAny' from assembly 'Microsoft.OpenApi, Version=2.3.0.0, Culture=neutral, PublicKeyToken=3f5743946376f042'.\r\nCould not load type 'Microsoft.OpenApi.Models.OpenApiDiscriminator' from assembly 'Microsoft.OpenApi, Version=2.3.0.0, Culture=neutral, PublicKeyToken=3f5743946376f042'.\r\nCould not load type 'Microsoft.OpenApi.Models.OpenApiExternalDocs' from assembly 'Microsoft.OpenApi, Version=2.3.0.0, Culture=neutral, PublicKeyToken=3f5743946376f042'.\r\nCould not load type 'Microsoft.OpenApi.Models.OpenApiReference' from assembly 'Microsoft.OpenApi, Version=2.3.0.0, Culture=neutral, PublicKeyToken=3f5743946376f042'.\r\nCould not load type 'Microsoft.OpenApi.Models.OpenApiSchema' from assembly 'Microsoft.OpenApi, Version=2.3.0.0, Culture=neutral, PublicKeyToken=3f5743946376f042'.\r\nCould not load type 'Microsoft.OpenApi.Models.OpenApiTag' from assembly 'Microsoft.OpenApi, Version=2.3.0.0, Culture=neutral, PublicKeyToken=..."}	System.Reflection.ReflectionTypeLoadException
*/