﻿namespace Player
{
    public class Character
    {
        public Character()
        {
            Inventory = new Inventory();
        }

        public Inventory Inventory { get; set; }
    }
}