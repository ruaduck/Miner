
namespace Tidus_Miner
{
    class Enums
    {
        public enum MiningColors : int
        {
            Reg = 0x0,
            DullCopper = 0x973,
            ShadowIron = 0x966,
            Copper = 0x96D,
            Bronze = 0x972,
            Golden = 0x8A5,
            Agapite = 0x979,
            Verite = 0x89F,
            Valorite = 0x8AB,
            Blaze = 0x489,
            Ice = 0x480,
            Toxic = 0x4F8,
            Electrum = 0x4FE,
            Platinum = 0x481,
            Magi = 0x47E,

        }

        public enum Ore : int
        {
            Large = 0x19B9,
            Medium = 0x19B8,
            Small = 0x19BA,
            Tiny = 0x19B7,               
        }

        public enum MiningEquipment : int
        {
            Shovel = 0xF39,
            Pickaxe = 0xE86,
        }
        public enum Ingots : int
        {
            Ingot = 0x1BF2,
        }
        public enum Forges : int
        {
            MobileForge = 0xE32,
            StaticForge = 0xFB1,
            OrcishForge = 0xFB1,
        }
        public enum RunebookGump : int
        {
            OSI = 0x0059,
            EVO = 0x554B87F3,
        }
    }
}
