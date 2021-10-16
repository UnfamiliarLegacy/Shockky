using System.Text;

using Shockky.IO;

namespace Shockky.Resources.Cast
{
    public class CastMemberMetadata : ShockwaveItem
    {
        public string ScriptText { get; set; }
        public string Name { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }

        public Guid XtraGUID { get; set; }
        public string XtraName { get; set; }

        public int[] RegistrationPoints { get; set; }

        public string ClipboardFormat { get; set; }

        public int CreationDate { get; set; }
        public int ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
        public string Comments { get; set; }
        
        public int ImageCompression { get; set; }
        public int ImageQuality { get; set; }

        public CastMemberMetadata(ref ShockwaveReader input)
        {
            int[] propertyOffsets = new int[input.ReadInt16() + 1];
            for (int i = 0; i < propertyOffsets.Length; i++)
            {
                propertyOffsets[i] = input.ReadInt32();
            }

            for (int i = 0; i < propertyOffsets.Length - 1; i++)
            {
                int length = propertyOffsets[i + 1] - propertyOffsets[i];
                if (length < 1) continue;

                ReadProperty(ref input, i, length);
            }
        }
        
        private void ReadProperty(ref ShockwaveReader input, int index, int length)
        {
            switch (index)
            {
                case 0:
                    ScriptText = input.ReadString(length);
                    break;
                case 1:
                    Name = input.ReadString();
                    break;
                case 2:
                    FilePath = input.ReadString(length);
                    break;
                case 3:
                    FileName = input.ReadString(length);
                    break;
                case 4:
                    FileType = input.ReadString(length);
                    break;
                case 5: // TODO: script rel? - string - prop 44
                    break;
                case 7: // TODO: prop 45 - string
                    string prop45 = input.ReadString(length);
                    break;
                case 9:
                    XtraGUID = new Guid(input.ReadBytes(length));
                    //XtraGUID = input.Read<Guid>();
                    break;
                case 10:
                    XtraName = input.ReadNullString();
                    break;
                case 11: //TODO:
                    break;
                case 12:
                    RegistrationPoints = new int[length / 4];
                    for (int i = 0; i < RegistrationPoints.Length; i++)
                    {
                        RegistrationPoints[i] = input.ReadInt32();
                    }
                    break;
                //15 - MoA ID?
                case 16:
                    ClipboardFormat = input.ReadString(length);
                    break;
                case 17:
                    CreationDate = input.ReadInt32() * 1000;
                    break;
                case 18:
                    ModifiedDate = input.ReadInt32() * 1000;
                    break;
                case 19:
                    ModifiedBy = input.ReadNullString();
                    break;
                case 20:
                    Comments = input.ReadString(length);
                    break;
                case 21:
                    ReadOnlySpan<byte> imageFlags = input.ReadBytes(length); //4

                    ImageCompression = imageFlags[0] >> 4;
                    ImageQuality = imageFlags[1];
                    break;
                default:
                    ReadOnlySpan<byte> unknown = input.ReadBytes(length);
                    string test = Encoding.UTF8.GetString(unknown);
                    break;
            }
        }

        public override int GetBodySize() => throw new NotImplementedException();
        public override void WriteTo(ShockwaveWriter output) => throw new NotImplementedException();
    }
}
