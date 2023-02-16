using System;
using System.Collections.Generic;
using System.Text;

namespace EternalKingdoom
{
    public class JsonCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Text { get; set; }
        public List<string> Choices { get; set; }
        public List<int> firstChoice { get; set; }
        public List<int> secondChoice { get; set; }
        public int cardType { get; set; }
    }
}