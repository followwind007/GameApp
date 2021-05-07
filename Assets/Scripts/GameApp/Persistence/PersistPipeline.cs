using System.Threading.Tasks;

namespace GameApp.Pesistence
{
    public struct PersistProfile
    {
        public enum PersistType { Default, Data }
        
        public PersistPipeline.AsyncCoder AsyncEncoder { get; }
        public PersistPipeline.AsyncCoder AsyncDecoder { get; }

        public PersistType PType { get; }

        public PersistProfile(PersistPipeline.AsyncCoder encoder, PersistPipeline.AsyncCoder decoder, PersistType type)
        {
            AsyncEncoder = encoder;
            AsyncDecoder = decoder;
            PType = type;
        }
    }
    
    public class PersistPipeline
    {
        public static readonly PersistProfile RawDataPipeline = new PersistProfile(
            AsyncEncodeRaw, 
            AsyncDecodeRaw, 
            PersistProfile.PersistType.Data);

        public delegate Task<byte[]> AsyncCoder(byte[] bytes);

        public static async Task<byte[]> AsyncEncodeRaw(byte[] bytes)
        {
            return await Task.FromResult(bytes);
        }
        public static async Task<byte[]> AsyncDecodeRaw(byte[] bytes)
        {
            return await Task.FromResult(bytes);
        }
        
    }
}