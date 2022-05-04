
namespace Hsinpa.Utility.Algorithm {
    public class CollisionAlgorithm
    {
        public static bool AABBCollision(GeneralDataStructure.AreaStruct area_a, GeneralDataStructure.AreaStruct area_b) {   
            return (area_a.x < area_b.x + area_b.width &&
                    area_a.x + area_a.width > area_b.x &&
                    area_a.y < area_b.y + area_b.height &&
                    area_a.height + area_a.y > area_b.y);
        }
    }
}