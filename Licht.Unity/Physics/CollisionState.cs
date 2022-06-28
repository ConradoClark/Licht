namespace Licht.Unity.Physics
{
    public struct CollisionState
    {
        public CollisionResult Right;
        public CollisionResult Left;
        public CollisionResult Up;
        public CollisionResult Down;

        public CollisionResult[] Custom;
    }
}
