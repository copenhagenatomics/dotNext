using System.Runtime.InteropServices;

namespace RaftNode;

struct sensorData {
    public int magicvalA;
    public int index;
    public int magicvalB;
    
    public static byte[] getBytes(sensorData str) 
    {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(str, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    public static sensorData fromBytes(byte[] arr) {
        sensorData str = new sensorData();

        int size = Marshal.SizeOf(str);
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, ptr, size);

        str = (sensorData)Marshal.PtrToStructure(ptr, str.GetType());
        Marshal.FreeHGlobal(ptr);

        return str;
    }


}