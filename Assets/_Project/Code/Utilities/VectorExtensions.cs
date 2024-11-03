namespace Code.Utilities
{
    public static class VectorExtensions
    {
        public static UnityEngine.Vector3 WithX(this UnityEngine.Vector3 vector, float x)
        {
            return new UnityEngine.Vector3(x, vector.y, vector.z);
        }
        
        public static UnityEngine.Vector3 WithY(this UnityEngine.Vector3 vector, float y)
        {
            return new UnityEngine.Vector3(vector.x, y, vector.z);
        }
        
        public static UnityEngine.Vector3 WithZ(this UnityEngine.Vector3 vector, float z)
        {
            return new UnityEngine.Vector3(vector.x, vector.y, z);
        }
    }
}