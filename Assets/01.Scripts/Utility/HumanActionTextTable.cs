public static class HumanActionTextTable
{
    public const string EatingFood = "열매 먹고 있는 중";
    public const string MovingToEatFood = "열매를 먹으러 가는 중";

    public const string MovingToGatherFood = "열매를 따러가는 중";
    public const string GatheringFood = "열매를 따는 중";
    public const string DeliveringFood = "열매를 저장소로 옮기는 중";

    public const string MovingToGatherWood = "나무를 캐러가는 중";
    public const string GatheringWood = "나무를 캐는 중";
    public const string DeliveringWood = "나무를 저장소로 옮기는 중";

    public const string Wandering = "돌아다니는 중";
    public const string Resting = "가만히 휴식 중";

    public const string MovingToBuildHouse = "집을 지으러 가는 중";
    public const string BuildingHouse = "집을 짓는 중";
    public const string MovingToHouse = "집으로 가는 중";
    public const string MovingToReproduce = "집으로 돌아가는 중";
    public const string Reproducing = "아이를 낳는 중";




    public static string GetMovingToGatherText(ResourceType resourceType)
    {
        return resourceType == ResourceType.Food ? MovingToGatherFood : MovingToGatherWood;
    }

    public static string GetGatheringText(ResourceType resourceType)
    {
        return resourceType == ResourceType.Food ? GatheringFood : GatheringWood;
    }

    public static string GetDeliveringText(ResourceType resourceType)
    {
        return resourceType == ResourceType.Food ? DeliveringFood : DeliveringWood;
    }
}
