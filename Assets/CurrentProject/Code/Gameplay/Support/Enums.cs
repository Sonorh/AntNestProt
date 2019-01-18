namespace PM.Antnest.Gameplay
{
    public enum Weeks
    {
        JanuaryFirst,
        JanuarySecond,
        JanuaryThird,
        JanuaryFourth,

        FebruaryFirst,
        FebruarySecond,
        FebruaryThird,
        FebruaryFourth,

        MarchFirst,
        MarchSecond,
        MarchThird,
        MarchFourth,

        AprilFirst,
        AprilSecond,
        AprilThird,
        AprilFourth,

        MayFirst,
        MaySecond,
        MayThird,
        MayFourth,

        JuneFirst,
        JuneSecond,
        JuneThird,
        JuneFourth,

        JulyFirst,
        JulySecond,
        JulyThird,
        JulyFourth,

        AugustFirst,
        AugustSecond,
        AugustThird,
        AugustFourth,

        SeptemberFirst,
        SeptemberSecond,
        SeptemberThird,
        SeptemberFourth,

        OctoberFirst,
        OctoberSecond,
        OctoberThird,
        OctoberFourth,

        NovemberFirst,
        NovemberSecond,
        NovemberThird,
        NovemberFourth,

        DecemberFirst,
        DecemberSecond,
        DecemberThird,
        DecemberFourth,
    }

    public enum AntTypes
    {
        Queen,
		Larva,
        Drone,
        Worker,
        Warrior,
        Scout,
        Gatherer,
        Builder,
        Nurse,
        Bruiser,
        Stopper
    }

    public enum PlanTypes
    {
        Gather,
		Hunt,
		Build,
		CreateUnit,
		Fight,
		Scout,
		Going,
		BuildRoom,
		UpgradeRoom
    }

    public enum ResourceTypes
    {
        Meat,
        Fruit
    }

    public enum SourceTypes
    {
        None,
        FlimsyMeat,
        FlimsyFruit,
        SolidMeat,
        SolidFruit
    }

    public enum BugTypes
    {
        None,
        Caterpillar,
        Spider,
        LadyBug
    }

	public enum RoomTypes
	{
		Entrance = 0,
		BuildingSpot = 1,
		Construction = 2,
		Warehause = 3,
		Nursery = 4,
		Armory = 5
	}
}