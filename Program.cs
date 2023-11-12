using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        string connectionString = "Data Source=db/DB_BANK_PAIVA.db";

        InitializeDatabase(connectionString);

        HashSet<BankAccount> accounts = new HashSet<BankAccount>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Sistema de Contas Bancárias");
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Criar conta");
            Console.WriteLine("2 - Depositar dinheiro");
            Console.WriteLine("3 - Sacar dinheiro");
            Console.WriteLine("4 - Verificar saldo");
            Console.WriteLine("5 - Listar contas");
            Console.WriteLine("6 - Sair");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    CreateAccount(accounts);
                    break;
                case "2":
                    DepositMoney(accounts);
                    break;
                case "3":
                    WithdrawMoney(accounts);
                    break;
                case "4":
                    CheckBalance(accounts);
                    break;
                case "5":
                    ListAccounts(accounts);
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }

    static void InitializeDatabase(string connectionString)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand("CREATE TABLE IF NOT EXISTS Accounts (Number INT PRIMARY KEY, Holder TEXT, Balance REAL);", connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    static void CreateAccount(HashSet<BankAccount> accounts)
    {
        Console.Write("Informe o nome do titular da conta: ");
        string holder = Console.ReadLine();

        Console.Write("Informe o saldo inicial da conta: ");
        decimal initialBalance = decimal.Parse(Console.ReadLine());

        int number = GenerateAccountNumber();
        BankAccount newAccount = new BankAccount(number, holder, initialBalance);

        accounts.Add(newAccount);
        InsertAccountIntoDatabase(newAccount);

        Console.WriteLine("Conta criada com sucesso.");
    }

    static void DepositMoney(HashSet<BankAccount> accounts)
    {
        Console.Write("Informe o número da conta para depósito: ");
        int accountNumber = int.Parse(Console.ReadLine());

        BankAccount account = FindAccount(accounts, accountNumber);
        if (account != null)
        {
            Console.Write("Informe o valor a ser depositado: ");
            decimal amount = decimal.Parse(Console.ReadLine());
            if (amount > 0)
            {
                account.Balance += amount;
                Console.WriteLine($"Depósito de {amount:C2} realizado com sucesso.");
            }
            else
            {
                Console.WriteLine("O valor do depósito deve ser maior que zero.");
            }
        }
        else
        {
            Console.WriteLine("Conta não encontrada.");
        }
    }

    static void WithdrawMoney(HashSet<BankAccount> accounts)
    {
        Console.Write("Informe o número da conta para saque: ");
        int accountNumber = int.Parse(Console.ReadLine());

        BankAccount account = FindAccount(accounts, accountNumber);
        if (account != null)
        {
            Console.Write("Informe o valor a ser sacado: ");
            decimal amount = decimal.Parse(Console.ReadLine());
            if (amount > 0 && amount <= account.Balance)
            {
                account.Balance -= amount;
                Console.WriteLine($"Saque de {amount:C2} realizado com sucesso.");
            }
            else
            {
                Console.WriteLine("Valor inválido ou saldo insuficiente.");
            }
        }
        else
        {
            Console.WriteLine("Conta não encontrada.");
        }
    }

    static void CheckBalance(HashSet<BankAccount> accounts)
    {
        Console.Write("Informe o número da conta para verificar o saldo: ");
        int accountNumber = int.Parse(Console.ReadLine());

        BankAccount account = FindAccount(accounts, accountNumber);
        if (account != null)
        {
            Console.WriteLine($"Saldo da conta de {account.Holder}: {account.Balance:C2}");
        }
        else
        {
            Console.WriteLine("Conta não encontrada.");
        }
    }

    static void ListAccounts(HashSet<BankAccount> accounts)
    {
        Console.WriteLine("Contas existentes:");
        foreach (var account in accounts)
        {
            Console.WriteLine($"Número da Conta: {account.Number} - Titular: {account.Holder} — Saldo: {account.Balance:C2}");
        }
    }

    static BankAccount FindAccount(HashSet<BankAccount> accounts, int accountNumber)
    {
        foreach (var account in accounts)
        {
            if (account.Number == accountNumber)
            {
                return account;
            }
        }
        return null;
    }

    static int GenerateAccountNumber()
    {
        Random random = new Random();
        return random.Next(1000, 2000);
    }

    static void InsertAccountIntoDatabase(BankAccount account)
    {
        string connectionString = "Data Source=db/DB_BANK_PAIVA.db";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand("INSERT INTO Accounts (Number, Holder, Balance) VALUES (@Number, @Holder, @Balance);", connection))
            {
                command.Parameters.AddWithValue("@Number", account.Number);
                command.Parameters.AddWithValue("@Holder", account.Holder);
                command.Parameters.AddWithValue("@Balance", account.Balance);

                command.ExecuteNonQuery();
            }
        }
    }
}

class BankAccount
{
    public int Number { get; }
    public string Holder { get; }
    public decimal Balance { get; set; }

    public BankAccount(int number, string holder, decimal balance)
    {
        Number = number;
        Holder = holder;
        Balance = balance;
    }
}
