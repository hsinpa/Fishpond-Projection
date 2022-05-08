
using UnityEngine;

namespace Hsinpa.Utility.Algorithm {
    public class CollisionAlgorithm
    {
        public static bool AABBIntersect(GeneralDataStructure.AreaStruct area_a, GeneralDataStructure.AreaStruct area_b) {   
            return (area_a.x < area_b.x + area_b.width &&
                    area_a.x + area_a.width > area_b.x &&
                    area_a.y < area_b.y + area_b.height &&
                    area_a.height + area_a.y > area_b.y);
        }

        public static bool BoxCircleIntersect(Vector4 rect, Vector3 circle)
        {
            float circleDistanceX = Mathf.Abs(circle.x - rect.x);
            float circleDistanceY = Mathf.Abs(circle.y - rect.y);

            if (circleDistanceX > (rect.z / 2f + circle.z)) { return false; }
            if (circleDistanceY > (rect.w / 2f + circle.z)) { return false; }

            if (circleDistanceX <= (rect.z / 2f)) { return true; }
            if (circleDistanceY <= (rect.w / 2f)) { return true; }

            float cornerDistance_sq = Mathf.Pow(circleDistanceX - rect.z / 2f , 2) +
                                     Mathf.Pow(circleDistanceY - rect.w / 2f , 2);

            return (cornerDistance_sq <= (circle.z * circle.z));
        }
    }
}