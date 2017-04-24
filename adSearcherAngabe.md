# AD Searcher
Es soll eine WPF Applikation erstellt werden, die aus unserem Active Directory alle Schüler/innen der eingegebenen Klasse auflistet. Es sollen Vorname, Zuname und Emailadresse angegeben werden. Der Zugriff auf das AD soll dabei über *async* Methoden, die *await* verwenden, erfolgen.

## Vorgehensweise
Um auf das Active Directory zuzugreifen, kannst du von [https://github.com/schletz/AdLibrary] eine fertige Bibliothek herunterladen. Öffne Die Solution `AcdLibrary.sln` und ergänze diese um ein neues WPF Projekt (Menüpunkt *Add Project*). Die Solution besitzt neben der DLL auch eine Konsolenapplikation (`AdLibrary.App`), die den Einsatz der Bibliothek zeigt.

In der Konsolenapplikation werden allerdings alle Aufrufe synchron durchgeführt. Bei langer Antwortzeit ist dies für eine WPF Appkilation daher eine schlechte Lösung. Um die WPF Applikation nicht einfrieren zu lassen, verwende daher für die `Authenticate` und `FindGroupMembers` Methoden das Schlüsselwort `await`.

## Aufruf von Methoden mit await
```
class Testclass
{
    public string Testmethod(string param)
    {
        return param;
    }
}
class Program
{
    static void Main(string[] args)
    {
        CallMethod();
        Console.ReadKey();
    }
    /// <summary>
    /// Dies ist eine Methode, die mit await auf das Ergebnis eines Tasks wartet.
    /// Diese Methode muss im Prototypen mit dem Schlüsselwort async definiert werden.
    /// </summary>
    static async void CallMethod()
    {
        Testclass t = new Testclass();
        /* Da Testmethod eine "normale", also synchrone, Methode ist,
            * packen wir sie mit Task.Run in einen Task und warten auf den
            * Rückgabewert. Beachte das Auslesen des Rückgabewertes als 
            * einfache Zuweisung! */
        string result = await Task.Run(() => t.Testmethod("Hello World."));

        Console.WriteLine(result);
    }
}
```
## Aussehen der GUI
Die GUI muss mehrere Elemente haben: Ein Textfeld für den Benutzernamen, eine PasswordBox für das Passwort und einen Login Button. Da Abfragen nur mit gültigen Zugangsdaten möglich sind, sind diese Felder notwendig.

Nach erfolgreichem Login wird das Eingabefeld für die Klasse und der Suchbutton aktiviert. Die gelesenen Schülerdaten sollen in einem Datagrid dargestellt werden.

