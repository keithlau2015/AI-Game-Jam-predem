using System.Collections.Generic;

namespace AttributeModule
{
    public interface IAttributeHolder
    {
        Dictionary<int, AttributeData> attributes { get; }
    }
}