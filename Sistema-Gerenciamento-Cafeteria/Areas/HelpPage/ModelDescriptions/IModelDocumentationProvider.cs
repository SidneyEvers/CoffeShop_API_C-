using System;
using System.Reflection;

namespace Sistema_Gerenciamento_Cafeteria.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}