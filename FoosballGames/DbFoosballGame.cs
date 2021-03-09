using System;

namespace FoosballGames
{
    public class DbFoosballGame
    {
        public Guid Id { get; private set; }
        public string JsonContent { get; private set; }

        public DbFoosballGame(Guid id, string jsonContent)
        {
            Id = id;
            JsonContent = jsonContent;
        }

        public void UpdateContent(string content)
        {
            JsonContent = content;
        }
    }
}