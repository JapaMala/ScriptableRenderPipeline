using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph
{
    [Title("Input", "Texture", "Texture 2D Texel Size")]
    public class Texture2DTexelSizeNode : AbstractMaterialNode, IGeneratesBodyCode, IMayRequireMeshUV
    {
        public const int OutputSlotISId = 1;
        public const int OutputSlotSId = 2;
        public const int TextureInputId = 0;

        const string kOutputSlotISName = "1/Size";
        const string kOutputSlotSName = "Size";
        const string kTextureInputName = "Texture";

        public override bool hasPreview { get { return false; } }

        public Texture2DTexelSizeNode()
        {
            name = "Texture 2D Texel Size";
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL
        {
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Texture-2D-Texel-Size-Node"; }
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector2MaterialSlot(OutputSlotISId, kOutputSlotISName, kOutputSlotISName, SlotType.Output, Vector2.one, ShaderStageCapability.Fragment));
            AddSlot(new Vector2MaterialSlot(OutputSlotSId, kOutputSlotSName, kOutputSlotSName, SlotType.Output, Vector2.one, ShaderStageCapability.Fragment));
            AddSlot(new Texture2DInputMaterialSlot(TextureInputId, kTextureInputName, kTextureInputName));
            RemoveSlotsNameNotMatching(new[] { OutputSlotISId, OutputSlotSId, TextureInputId });
        }

        // Node generations
        public virtual void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {
            var id = GetSlotValue(TextureInputId, generationMode) + "_TexelSize";

            visitor.AddShaderChunk(string.Format("{0}2 {1} = {2}.xy;", precision, GetVariableNameForSlot(OutputSlotISId), id), true);
            visitor.AddShaderChunk(string.Format("{0}2 {1} = {2}.zw;", precision, GetVariableNameForSlot(OutputSlotSId), id), true);
        }

        public bool RequiresMeshUV(UVChannel channel, ShaderStageCapability stageCapability)
        {
            s_TempSlots.Clear();
            GetInputSlots(s_TempSlots);
            foreach (var slot in s_TempSlots)
            {
                if (slot.RequiresMeshUV(channel))
                    return true;
            }
            return false;
        }
    }
}
