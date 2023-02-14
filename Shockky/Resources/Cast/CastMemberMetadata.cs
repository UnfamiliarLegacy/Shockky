using System.Text;

using Shockky.IO;

namespace Shockky.Resources.Cast;

public sealed class CastMemberMetadata : IResource, IShockwaveItem
{
    public OsType Kind => OsType.VWCI;

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

    public CastMemberMetadata(ref ShockwaveReader input, ReaderContext context)
    {
        // TODO: VList
        input.ReadInt32();
        int script_garbage_ptr = input.ReadInt32(); //script_ptr
        input.ReadInt32(); // csnover: legacy event bitflag maybe
        int flags = input.ReadInt32(); //flags
        int script_ctxt_num = input.ReadInt32(); //script_context_num 

        int[] propertyOffsets = new int[input.ReadInt16() + 1];
        for (int i = 0; i < propertyOffsets.Length; i++)
        {
            propertyOffsets[i] = input.ReadInt32();
        }

        throw new NotImplementedException();
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
                //TODO: MoaRect - TLBR
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

    public int GetBodySize(WriterOptions options)
    {
        throw new NotImplementedException();
    }
    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        throw new NotImplementedException();
    }
}
