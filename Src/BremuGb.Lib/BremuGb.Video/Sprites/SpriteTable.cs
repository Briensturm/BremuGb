using System;

namespace BremuGb.Video.Sprites
{
    class SpriteTable
    {
        internal Sprite[] Sprites { get; }

        public SpriteTable()
        {
            Sprites = new Sprite[40];
            for (int i = 0; i < Sprites.Length; i++)
            {
                Sprites[i] = new Sprite();
                Sprites[i].OamIndex = i;
            }
        }

        public void WriteSpriteAttributeTable(ushort address, byte data)
        {
            //determine sprite number
            int spriteNumber = (address - 0xFE00) / 4;
            int attributeNumber = (address - 0xFE00) % 4;

            switch(attributeNumber)
            {
                case 0:
                    Sprites[spriteNumber].PositionY = data;
                    break;
                case 1:
                    Sprites[spriteNumber].PositionX = data;
                    break;
                case 2:
                    Sprites[spriteNumber].TileNumber = data;
                    break;
                case 3:
                    Sprites[spriteNumber].Flags = data;
                    break;
            }            
        }

        public byte ReadSpriteAttributeTable(ushort address)
        {
            //determine sprite number
            int spriteNumber = (address - 0xFE00) / 4;
            int attributeNumber = (address - 0xFE00) % 4;

            return attributeNumber switch
            {
                0 => Sprites[spriteNumber].PositionY,
                1 => Sprites[spriteNumber].PositionX,
                2 => Sprites[spriteNumber].TileNumber,
                3 => Sprites[spriteNumber].Flags,
                _ => throw new InvalidOperationException($"Invalid sprite attribute number {attributeNumber}"),
            };
        }
    }
}
