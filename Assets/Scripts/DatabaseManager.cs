using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public UserDataController userDataController;
    public StartNewOrder startNewOrder;

    public int userGold = 0;
    public int meat = 0;
    public int iron = 0;
    public int herbs = 0;

    void Start()
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();

        dbCommandReadValues.CommandText = "SELECT Gold FROM Economy";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        while (dataReader.Read())
        {
            userGold = dataReader.GetInt32(0);
        }
        dataReader.Close();

        dbCommandReadValues.CommandText = "SELECT Quantity FROM Resources";
        dataReader = dbCommandReadValues.ExecuteReader();
        List<int> resourcesData = new List<int>();
        while (dataReader.Read())
        {
            resourcesData.Add(dataReader.GetInt32(0));
        }
        if (resourcesData.Count == 3)
        {
            meat = resourcesData[0];
            iron = resourcesData[1];
            herbs = resourcesData[2];
        }
        dataReader.Close();

        Dictionary<string, string> adventurersDictionary = GetAdventurersDictionary(dbConnection);
        Dictionary<string, int> resourcesDictionary = GetResourcesDictionary(dbConnection);
        startNewOrder.GetPersonLists(adventurersDictionary, resourcesDictionary);

        userDataController.UpdateAllText();
    }

    //private void OnMouseDown()
    //{
    //    hitCount++;
    //    IDbConnection dbConnection = CreateAndOpenDatabase();
    //    IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
    //    dbCommandInsertValue.CommandText = "INSERT OR REPLACE INTO HitCountTableSimple (id, hits) VALUES (0, " + hitCount + ")";
    //    dbCommandInsertValue.ExecuteNonQuery();
    //    dbConnection.Close();
    //}

    private IDbConnection CreateAndOpenDatabase()
    {
        string dbUri = "URI=file:" + Application.dataPath + "/StreamingAssets/Taverna.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();
        CreateTables(dbConnection);
        return dbConnection;
    }

    public void CreateTables(IDbConnection dbConnection)
    {
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();

        string createAllTables = @"
            CREATE TABLE IF NOT EXISTS Adventurers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AdventurerId INTEGER NOT NULL,
                OrderType TEXT NOT NULL,
                IsCompleted INTEGER NOT NULL,
                FOREIGN KEY (AdventurerId) REFERENCES Adventurers(Id)
            );

            CREATE TABLE IF NOT EXISTS Resources (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ResourceName TEXT NOT NULL,
                Quantity INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Economy (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Gold INTEGER NOT NULL
            );";

        dbCommandCreateTable.CommandText = createAllTables;
        dbCommandCreateTable.ExecuteReader();
    }

    private void SetNewOrders(IDbConnection dbConnection, int adventurerId, string resourceName, int quantity)
    {
        IDbCommand dbCommandSetNewOrders = dbConnection.CreateCommand();
        dbCommandSetNewOrders.CommandText = @"
        INSERT INTO Orders (OrderType, IsCompleted) 
        VALUES (@adventurerId, @orderType, 0)";
        IDbDataParameter parameterAdventurerId = dbCommandSetNewOrders.CreateParameter();
        parameterAdventurerId.ParameterName = "@adventurerId";
        parameterAdventurerId.Value = adventurerId;

        IDbDataParameter parameterOrderType = dbCommandSetNewOrders.CreateParameter();
        parameterOrderType.ParameterName = "@orderType";
        parameterOrderType.Value = $"{quantity}x {resourceName}";

        dbCommandSetNewOrders.Parameters.Add(parameterAdventurerId);
        dbCommandSetNewOrders.Parameters.Add(parameterOrderType);

        dbCommandSetNewOrders.ExecuteNonQuery();
    }

    private Dictionary<string, string> GetAdventurersDictionary(IDbConnection dbConnection)
    {
        Dictionary<string, string> adventurersDictionary = new Dictionary<string, string>();
        IDbCommand dbCommandReadAdventurers = dbConnection.CreateCommand();
        dbCommandReadAdventurers.CommandText = "SELECT Name, Type FROM Adventurers";

        IDataReader dataReader = dbCommandReadAdventurers.ExecuteReader();
        while (dataReader.Read())
        {
            string name = dataReader.GetString(0);
            string type = dataReader.GetString(1);
            adventurersDictionary[name] = type;
        }
        dataReader.Close();

        return adventurersDictionary;
    }

    private Dictionary<string, int> GetResourcesDictionary(IDbConnection dbConnection)
    {
        Dictionary<string, int> resourcesDictionary = new Dictionary<string, int>();
        IDbCommand dbCommandReadAdventurers = dbConnection.CreateCommand();
        dbCommandReadAdventurers.CommandText = "SELECT ResourceName, Quantity FROM Resources";

        IDataReader dataReader = dbCommandReadAdventurers.ExecuteReader();
        while (dataReader.Read())
        {
            string name = dataReader.GetString(0);
            int count = dataReader.GetInt32(1);
            resourcesDictionary[name] = count;
        }
        dataReader.Close();

        return resourcesDictionary;
    }

    //    INSERT INTO Adventurers(Name, Type, Level, Reputation) VALUES
    //      ('Sir Lancelot', 'Knight', 5, 10),
    //      ('Merlin', 'Mage', 7, 15),
    //      ('Robin', 'Rogue', 4, 8);
    //    INSERT INTO Resources(ResourceName, Quantity) VALUES
    //      ('Meat', 10),
    //      ('Vegetables', 15),
    //      ('Potions', 5);
    //    INSERT INTO Economy(Gold) VALUES(1000);
}