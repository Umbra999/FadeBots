using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FadeBot
{
    public static class CustomTypes
    {
        private static LoadBalancingClient Client;

        public static void Register(LoadBalancingClient client)
        {
            Client = client;
            Type typeFromHandle = typeof(Vector3);
            byte code = 86;
            if (SerializeVector3 == null)
            {
                SerializeVector3 = new SerializeMethod(Vector3SerializeMethod);
            }
            SerializeMethod serializeVector = SerializeVector3;
            if (DeserializeVector3 == null)
            {
                DeserializeVector3 = new DeserializeMethod(Vector3DeserializeMethod);
            }
            PhotonPeer.RegisterType(typeFromHandle, code, serializeVector, DeserializeVector3);
            Type typeFromHandle2 = typeof(Quaternion);
            byte code2 = 81;
            if (SerializeQuaternion == null)
            {
                SerializeQuaternion = new SerializeMethod(QuaternionSerializeMethod);
            }
            SerializeMethod serializeQuaternion = SerializeQuaternion;
            if (DeserializeQuaternion == null)
            {
                DeserializeQuaternion = new DeserializeMethod(QuaternionDeserializeMethod);
            }
            PhotonPeer.RegisterType(typeFromHandle2, code2, serializeQuaternion, DeserializeQuaternion);
            byte b = 100;
            Type typeFromHandle3 = typeof(Vector2);
            if (SerializeVector2 == null)
            {
                SerializeVector2 = new SerializeMethod(Vector2SerializeMethod);
            }
            SerializeMethod serializeVector2 = SerializeVector2;
            if (DeserializeVector2 == null)
            {
                DeserializeVector2 = new DeserializeMethod(Vector2DeserializeMethod);
            }
            IDK(typeFromHandle3, ref b, serializeVector2, DeserializeVector2);
            Type typeFromHandle4 = typeof(Vector4);
            if (SerializeVector4 == null)
            {
                SerializeVector4 = new SerializeMethod(Vector4SerializeMethod);
            }
            SerializeMethod serializeVector3 = SerializeVector4;
            if (DeserializeVector4 == null)
            {
                DeserializeVector4 = new DeserializeMethod(Vector4DeserializeMethod);
            }
            IDK(typeFromHandle4, ref b, serializeVector3, DeserializeVector4);
            PhotonPeer.RegisterType(typeof(Player), (byte)'P', SerializePhotonPlayer, DeserializePhotonPlayer);
        }

        private static void IDK(Type type, ref byte @byte, SerializeMethod serializemethod, DeserializeMethod deserializemethod)
        {
            byte code;
            @byte = (byte)((code = @byte) + 1);
            if (PhotonPeer.RegisterType(type, code, serializemethod, deserializemethod) && !types.Contains(type))
            {
                types.Add(type);
            }
        }

        internal static List<Type> types = new List<Type>
        {
            typeof(byte),
            typeof(bool),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(string)
        };

        private static object QuaternionDeserializeMethod(byte[] FBNNBMOEGOK)
        {
            int num = 0;
            float x;
            Protocol.Deserialize(out x, FBNNBMOEGOK, ref num);
            float y;
            Protocol.Deserialize(out y, FBNNBMOEGOK, ref num);
            float z;
            Protocol.Deserialize(out z, FBNNBMOEGOK, ref num);
            float w;
            Protocol.Deserialize(out w, FBNNBMOEGOK, ref num);
            return new Quaternion(x, y, z, w);
        }

        private static byte[] QuaternionSerializeMethod(object LIFOGCIHAMI)
        {
            byte[] array = new byte[16];
            int num = 0;
            Protocol.Serialize(((Quaternion)LIFOGCIHAMI).X, array, ref num);
            Protocol.Serialize(((Quaternion)LIFOGCIHAMI).Y, array, ref num);
            Protocol.Serialize(((Quaternion)LIFOGCIHAMI).Z, array, ref num);
            Protocol.Serialize(((Quaternion)LIFOGCIHAMI).W, array, ref num);
            return array;
        }

        private static object Vector2DeserializeMethod(byte[] FBNNBMOEGOK)
        {
            int num = 0;
            float x;
            Protocol.Deserialize(out x, FBNNBMOEGOK, ref num);
            float y;
            Protocol.Deserialize(out y, FBNNBMOEGOK, ref num);
            return new Vector2(x, y);
        }

        private static byte[] Vector2SerializeMethod(object LIFOGCIHAMI)
        {
            byte[] array = new byte[8];
            int num = 0;
            Protocol.Serialize(((Vector2)LIFOGCIHAMI).X, array, ref num);
            Protocol.Serialize(((Vector2)LIFOGCIHAMI).Y, array, ref num);
            return array;
        }

        private static object Vector3DeserializeMethod(byte[] FBNNBMOEGOK)
        {
            int num = 0;
            float x;
            Protocol.Deserialize(out x, FBNNBMOEGOK, ref num);
            float y;
            Protocol.Deserialize(out y, FBNNBMOEGOK, ref num);
            float z;
            Protocol.Deserialize(out z, FBNNBMOEGOK, ref num);
            return new Vector3(x, y, z);
        }

        private static byte[] Vector3SerializeMethod(object LIFOGCIHAMI)
        {
            byte[] array = new byte[12];
            int num = 0;
            Protocol.Serialize(((Vector3)LIFOGCIHAMI).X, array, ref num);
            Protocol.Serialize(((Vector3)LIFOGCIHAMI).Y, array, ref num);
            Protocol.Serialize(((Vector3)LIFOGCIHAMI).Z, array, ref num);
            return array;
        }

        private static object Vector4DeserializeMethod(byte[] FBNNBMOEGOK)
        {
            int num = 0;
            float x;
            Protocol.Deserialize(out x, FBNNBMOEGOK, ref num);
            float y;
            Protocol.Deserialize(out y, FBNNBMOEGOK, ref num);
            float z;
            Protocol.Deserialize(out z, FBNNBMOEGOK, ref num);
            float w;
            Protocol.Deserialize(out w, FBNNBMOEGOK, ref num);
            return new Vector4(x, y, z, w);
        }

        private static byte[] Vector4SerializeMethod(object LIFOGCIHAMI)
        {
            byte[] array = new byte[16];
            int num = 0;
            Protocol.Serialize(((Vector4)LIFOGCIHAMI).X, array, ref num);
            Protocol.Serialize(((Vector4)LIFOGCIHAMI).Y, array, ref num);
            Protocol.Serialize(((Vector4)LIFOGCIHAMI).Z, array, ref num);
            Protocol.Serialize(((Vector4)LIFOGCIHAMI).W, array, ref num);
            return array;
        }

        public static readonly byte[] memPlayer = new byte[4];

        private static short SerializePhotonPlayer(StreamBuffer outStream, object customobject)
        {
            var ID = ((Player)customobject).ActorNumber;

            lock (memPlayer)
            {
                var bytes = memPlayer;
                var off = 0;
                Protocol.Serialize(ID, bytes, ref off);
                outStream.Write(bytes, 0, 4);
                return 4;
            }
        }

        private static object DeserializePhotonPlayer(StreamBuffer inStream, short length)
        {
            int ID;
            lock (memPlayer)
            {
                inStream.Read(memPlayer, 0, length);
                var off = 0;
                Protocol.Deserialize(out ID, memPlayer, ref off);
            }

            if (Client.CurrentRoom != null)
            {
                var player = Client.CurrentRoom.GetPlayer(ID);
                return player;
            }

            return null;
        }

        private static SerializeMethod SerializeVector3;
        private static DeserializeMethod DeserializeVector3;
        private static SerializeMethod SerializeQuaternion;
        private static DeserializeMethod DeserializeQuaternion;
        private static SerializeMethod SerializeVector2;
        private static DeserializeMethod DeserializeVector2;
        private static SerializeMethod SerializeVector4;
        private static DeserializeMethod DeserializeVector4;

        public class RPC
        {
            public string rpcName;
            public string parameterString;
            public VrcEventType eventType;
            public VrcBroadcastType broadcastType;
            public byte[] parameterBytes;

            public static byte[] Serialize(object customType)
            {
                return new byte[32768];
            }

            public static object Deserialize(byte[] data)
            {
                RPC rpc = new RPC();
                int num = 0;
                float num2;
                Protocol.Deserialize(out num2, data, ref num);
                int num3;
                Protocol.Deserialize(out num3, data, ref num);
                short num4;
                Protocol.Deserialize(out num4, data, ref num);
                rpc.rpcName = Encoding.UTF8.GetString(data, num, num4);
                num += num4;
                int num5;
                Protocol.Deserialize(out num5, data, ref num);
                rpc.eventType = (VrcEventType)Enum.ToObject(typeof(VrcEventType), num5);
                short num6;
                Protocol.Deserialize(out num6, data, ref num);
                int num7;
                Protocol.Deserialize(out num7, data, ref num);
                float num8;
                Protocol.Deserialize(out num8, data, ref num);
                int num9;
                Protocol.Deserialize(out num9, data, ref num);
                Protocol.Deserialize(out num4, data, ref num);
                rpc.parameterString = Encoding.UTF8.GetString(data, num, num4);
                num += num4;
                try
                {
                    float num10;
                    Protocol.Deserialize(out num10, data, ref num);
                    short num11;
                    Protocol.Deserialize(out num11, data, ref num);
                    rpc.broadcastType = (VrcBroadcastType)Enum.ToObject(typeof(VrcBroadcastType), num11);
                    short num12;
                    Protocol.Deserialize(out num12, data, ref num);
                    rpc.parameterBytes = new byte[num12];
                    if (num12 > 0)
                        Array.Copy(data, num, rpc.parameterBytes, 0, num12);
                    num += num12;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    rpc.parameterBytes = new byte[0];
                }
                Console.WriteLine($"RPC Received | Target {rpc.rpcName} : ParamString {(string.IsNullOrEmpty(rpc.parameterString) ? "-empty-" : rpc.parameterString)} : EvType {rpc.eventType} : BroadcType {rpc.broadcastType} : SizeOfParamsBytes {rpc.parameterBytes.Length}");
                return rpc;
            }
        }

        public enum VrcEventType
        {
            // Token: 0x0400006A RID: 106
            MeshVisibility,

            // Token: 0x0400006B RID: 107
            AnimationFloat,

            // Token: 0x0400006C RID: 108
            AnimationBool,

            // Token: 0x0400006D RID: 109
            AnimationTrigger,

            // Token: 0x0400006E RID: 110
            AudioTrigger,

            // Token: 0x0400006F RID: 111
            PlayAnimation,

            // Token: 0x04000070 RID: 112
            SendMessage,

            // Token: 0x04000071 RID: 113
            SetParticlePlaying,

            // Token: 0x04000072 RID: 114
            TeleportPlayer,

            // Token: 0x04000073 RID: 115
            RunConsoleCommand,

            // Token: 0x04000074 RID: 116
            SetGameObjectActive,

            // Token: 0x04000075 RID: 117
            SetWebPanelURI,

            // Token: 0x04000076 RID: 118
            SetWebPanelVolume,

            // Token: 0x04000077 RID: 119
            SpawnObject,

            // Token: 0x04000078 RID: 120
            SendRPC,

            // Token: 0x04000079 RID: 121
            ActivateCustomTrigger,

            // Token: 0x0400007A RID: 122
            DestroyObject,

            // Token: 0x0400007B RID: 123
            SetLayer,

            // Token: 0x0400007C RID: 124
            SetMaterial,

            // Token: 0x0400007D RID: 125
            AddHealth,

            // Token: 0x0400007E RID: 126
            AddDamage,

            // Token: 0x0400007F RID: 127
            SetComponentActive,

            // Token: 0x04000080 RID: 128
            AnimationInt,

            // Token: 0x04000081 RID: 129
            AnimationIntAdd = 24,

            // Token: 0x04000082 RID: 130
            AnimationIntSubtract,

            // Token: 0x04000083 RID: 131
            AnimationIntMultiply,

            // Token: 0x04000084 RID: 132
            AnimationIntDivide,

            // Token: 0x04000085 RID: 133
            AddVelocity,

            // Token: 0x04000086 RID: 134
            SetVelocity,

            // Token: 0x04000087 RID: 135
            AddAngularVelocity,

            // Token: 0x04000088 RID: 136
            SetAngularVelocity,

            // Token: 0x04000089 RID: 137
            AddForce,

            // Token: 0x0400008A RID: 138
            SetUIText
        }

        public enum VrcBroadcastType
        {
            // Token: 0x0400008C RID: 140
            Always,

            // Token: 0x0400008D RID: 141
            Master,

            // Token: 0x0400008E RID: 142
            Local,

            // Token: 0x0400008F RID: 143
            Owner,

            // Token: 0x04000090 RID: 144
            AlwaysUnbuffered,

            // Token: 0x04000091 RID: 145
            MasterUnbuffered,

            // Token: 0x04000092 RID: 146
            OwnerUnbuffered,

            // Token: 0x04000093 RID: 147
            AlwaysBufferOne,

            // Token: 0x04000094 RID: 148
            MasterBufferOne,

            // Token: 0x04000095 RID: 149
            OwnerBufferOne
        }
    }
}