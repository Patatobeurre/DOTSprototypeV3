using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_ColorCol", MaterialPropertyFormat.Float4)]
    struct ColorColVector4Override : IComponentData
    {
        public float4 Value;
    }
}
