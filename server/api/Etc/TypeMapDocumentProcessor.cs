using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace api.Etc;

public class TypeMapDocumentProcessor<T> : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var schema = context.SchemaGenerator.Generate(typeof(T));
        context.Document.Definitions[typeof(T).Name] = schema;
    }
}