// Программа демонстрирует работу трех паттернов проектирования: Команда, Шаблонный метод и Посредник.
// Команда — управление устройствами умного дома с возможностью отмены действий.
// Шаблонный метод — приготовление напитков с общей структурой и уникальными шагами.
// Посредник — чат-комната, где пользователи общаются через посредника, а не напрямую.

using System;
using System.Collections.Generic;

interface ICommand { void Execute(); void Undo(); }

class Light { public void On() { Console.WriteLine("Свет включен"); } public void Off() { Console.WriteLine("Свет выключен"); } }
class Door { public void Open() { Console.WriteLine("Дверь открыта"); } public void Close() { Console.WriteLine("Дверь закрыта"); } }
class Thermostat { int t = 22; public void Increase() { t++; Console.WriteLine($"Температура повышена до {t}°C"); } public void Decrease() { t--; Console.WriteLine($"Температура понижена до {t}°C"); } }
class TV { public void On() { Console.WriteLine("Телевизор включен"); } public void Off() { Console.WriteLine("Телевизор выключен"); } }

class LightOnCommand : ICommand { Light l; public LightOnCommand(Light l) { this.l = l; } public void Execute() { l.On(); } public void Undo() { l.Off(); } }
class LightOffCommand : ICommand { Light l; public LightOffCommand(Light l) { this.l = l; } public void Execute() { l.Off(); } public void Undo() { l.On(); } }
class DoorOpenCommand : ICommand { Door d; public DoorOpenCommand(Door d) { this.d = d; } public void Execute() { d.Open(); } public void Undo() { d.Close(); } }
class DoorCloseCommand : ICommand { Door d; public DoorCloseCommand(Door d) { this.d = d; } public void Execute() { d.Close(); } public void Undo() { d.Open(); } }
class TempUpCommand : ICommand { Thermostat t; public TempUpCommand(Thermostat t) { this.t = t; } public void Execute() { t.Increase(); } public void Undo() { t.Decrease(); } }
class TempDownCommand : ICommand { Thermostat t; public TempDownCommand(Thermostat t) { this.t = t; } public void Execute() { t.Decrease(); } public void Undo() { t.Increase(); } }
class TVOnCommand : ICommand { TV tv; public TVOnCommand(TV tv) { this.tv = tv; } public void Execute() { tv.On(); } public void Undo() { tv.Off(); } }
class TVOffCommand : ICommand { TV tv; public TVOffCommand(TV tv) { this.tv = tv; } public void Execute() { tv.Off(); } public void Undo() { tv.On(); } }

class Invoker
{
    Stack<ICommand> history = new Stack<ICommand>();
    public void Execute(ICommand c) { c.Execute(); history.Push(c); }
    public void Undo()
    {
        if (history.Count > 0) history.Pop().Undo();
        else Console.WriteLine("Нет команд для отмены");
    }
}

// ---------- Шаблонный метод ----------
abstract class Beverage
{
    public void Prepare() { BoilWater(); Brew(); Pour(); if (CustomerWantsCondiments()) AddCondiments(); }
    void BoilWater() { Console.WriteLine("Кипятим воду"); }
    void Pour() { Console.WriteLine("Наливаем в чашку"); }
    protected abstract void Brew();
    protected abstract void AddCondiments();
    protected virtual bool CustomerWantsCondiments()
    {
        Console.Write("Добавить добавки (y/n)? ");
        string s = Console.ReadLine();
        return s == "y";
    }
}
class Tea : Beverage
{
    protected override void Brew() { Console.WriteLine("Завариваем чай"); }
    protected override void AddCondiments() { Console.WriteLine("Добавляем лимон"); }
}
class Coffee : Beverage
{
    protected override void Brew() { Console.WriteLine("Завариваем кофе"); }
    protected override void AddCondiments() { Console.WriteLine("Добавляем сахар и молоко"); }
}
class HotChocolate : Beverage
{
    protected override void Brew() { Console.WriteLine("Растапливаем шоколад"); }
    protected override void AddCondiments() { Console.WriteLine("Добавляем взбитые сливки"); }
}

// ---------- Посредник ----------
interface IMediator { void Send(string msg, User u); void Register(User u); }
class ChatRoom : IMediator
{
    List<User> users = new List<User>();
    public void Register(User u) { users.Add(u); NotifyJoin(u); }
    public void Send(string msg, User s)
    {
        foreach (var u in users)
            if (u != s) u.Receive(msg, s.Name);
    }
    void NotifyJoin(User u)
    {
        foreach (var other in users)
            if (other != u) other.Receive($"Пользователь {u.Name} присоединился к чату", "Система");
    }
}
class User
{
    protected IMediator chat;
    public string Name { get; }
    public User(IMediator c, string n) { chat = c; Name = n; }
    public void Send(string msg) { chat.Send(msg, this); }
    public void Receive(string msg, string from) { Console.WriteLine($"{from} -> {Name}: {msg}"); }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Паттерн Команда ===");
        var inv = new Invoker();
        var l = new Light();
        var d = new Door();
        var t = new Thermostat();
        var tv = new TV();

        inv.Execute(new LightOnCommand(l));
        inv.Execute(new DoorOpenCommand(d));
        inv.Execute(new TempUpCommand(t));
        inv.Execute(new TVOnCommand(tv));
        inv.Undo();
        inv.Undo();
        inv.Undo();
        inv.Undo();
        inv.Undo();

        Console.WriteLine("\n=== Паттерн Шаблонный метод ===");
        Beverage tea = new Tea();
        tea.Prepare();
        Beverage coffee = new Coffee();
        coffee.Prepare();
        Beverage choco = new HotChocolate();
        choco.Prepare();

        Console.WriteLine("\n=== Паттерн Посредник ===");
        ChatRoom chat = new ChatRoom();
        User u1 = new User(chat, "Алиса");
        User u2 = new User(chat, "Боб");
        User u3 = new User(chat, "Ева");
        chat.Register(u1);
        chat.Register(u2);
        chat.Register(u3);
        u1.Send("Привет всем!");
        u2.Send("Привет, Алиса!");
        u3.Send("Как дела?");
    }
}


// === Паттерн Команда ===
// Свет включен
// Дверь открыта
// Температура повышена до 23°C
// Телевизор включен
// Телевизор выключен
// Температура понижена до 22°C
// Дверь закрыта
// Свет выключен
// Нет команд для отмены

// === Паттерн Шаблонный метод ===
// Кипятим воду
// Завариваем чай
// Наливаем в чашку
// Добавить добавки (y/n)? y
// Добавляем лимон
// Кипятим воду
// Завариваем кофе
// Наливаем в чашку
// Добавить добавки (y/n)? n
// Кипятим воду
// Растапливаем шоколад
// Наливаем в чашку
// Добавить добавки (y/n)? n

// === Паттерн Посредник ===
// Система -> Алиса: Пользователь Боб присоединился к чату
// Система -> Алиса: Пользователь Ева присоединился к чату
// Система -> Боб: Пользователь Ева присоединился к чату
// Алиса -> Боб: Привет всем!
// Алиса -> Ева: Привет всем!
// Боб -> Алиса: Привет, Алиса!
// Боб -> Ева: Привет, Алиса!
// Ева -> Алиса: Как дела?
// Ева -> Боб: Как дела?
// PS C:\Users\User\Desktop\c#\CsLab>

/*

--- Команда ---
1. Преимущества: инкапсуляция действий, возможность отмены, простое добавление новых команд.
2. Новые команды добавляются созданием новых классов, не изменяя существующий код.
3. Отличие — команда превращает вызов метода в объект, что позволяет хранить, передавать и отменять действия.

--- Шаблонный метод ---
1. Преимущества: выделение общей логики, уменьшение дублирования кода, легкость расширения.
2. Добавить новые напитки можно созданием новых подклассов.
3. Hook — необязательный метод, который можно переопределить, чтобы изменить поведение, не ломая шаблон.

--- Посредник ---
1. Преимущества: уменьшение связей между объектами, централизованное управление взаимодействием.
2. Новые типы участников можно добавить через новые классы, не меняя существующие.
3. Изменив посредника, можно реализовать групповые или личные сообщения через хранение списков получателей.
*/
