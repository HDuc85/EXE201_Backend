using Data.ViewModel.Product;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(CreateProductDTO))
            .ToList();

        if (fileParams.Count > 0)
        {
            var uploadSchema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["productName"] = new OpenApiSchema { Type = "string" },
                    ["quantitySold"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                    ["description"] = new OpenApiSchema { Type = "string" },
                    ["productVariants"] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["sizeName"] = new OpenApiSchema { Type = "string" },
                                ["brandName"] = new OpenApiSchema { Type = "string" },
                                ["colorName"] = new OpenApiSchema { Type = "string" },
                                ["thumbnail"] = new OpenApiSchema { Type = "string", Format = "binary" },
                                ["price"] = new OpenApiSchema { Type = "number", Format = "double" },
                                ["quantity"] = new OpenApiSchema { Type = "integer", Format = "int32" }
                            }
                        }
                    },
                    ["mediaUrls"] = new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string" } }
                }
            };

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = uploadSchema
                    }
                }
            };
        }
    }
}
