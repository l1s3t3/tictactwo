using GameBrain;

namespace MenuSystem;

public class Menu
{
    public string MenuHeader { get; set; }
    private static string? _menuDivider = "===========================================";
    private List<MenuItem> MenuItems { get; set; }

    private MenuItem _menuItemExit = new MenuItem()
    {
        Shortcut = "E",
        Title = "Exit",
    };
    
    private MenuItem _menuItemReturn = new MenuItem()
    {
        Shortcut = "R",
        Title = "Return",
    };
    
    private MenuItem _menuItemReturnMain = new MenuItem()
    {
        Shortcut = "M",
        Title = "Return to main menu",
    };
    
    private EMenuLevel _menuLevel { get; set; }

    public void SetMenuItemAction(string shortCut, Func<string> action)
    {
       var menuItem = MenuItems.Single(m => m.Shortcut == shortCut);
       menuItem.MenuItemAction = action;
    }

    public Menu(EMenuLevel menuLevel, string menuHeader, List<MenuItem> menuItems)
    {
        if (string.IsNullOrWhiteSpace(menuHeader))
        {
            throw new ApplicationException("Menu header cannot be null or empty.");
        }
        
        MenuHeader = menuHeader;
        
        if (menuItems == null || menuItems.Count == 0)
        {
            throw new ApplicationException("Menu items cannot be null or empty.");
        }
        
        MenuItems = menuItems;
        _menuLevel = menuLevel;
        
        if (_menuLevel != EMenuLevel.Main)
        {
            MenuItems.Add(_menuItemReturn);
        }
        
        MenuItems.Add(_menuItemExit);
        
    }

    public string Run()
    {
        Console.Clear();

        do
        {
            var menuItem = DisplayMenuGetUserChoice();
            var menuReturnValue = "";

            if (menuItem.MenuItemAction != null)
            {
                menuReturnValue = menuItem.MenuItemAction();
            }

            // Check if the user wants to exit or return to main menu
            if (menuItem.Shortcut == _menuItemExit.Shortcut || menuReturnValue == _menuItemExit.Shortcut)
            {
                return _menuItemExit.Shortcut;
            }

            if (menuItem.Shortcut == _menuItemReturn.Shortcut)
            {
                return _menuItemReturn.Shortcut; // Go back to previous menu
            }

            if ((menuItem.Shortcut == _menuItemReturnMain.Shortcut) && _menuLevel != EMenuLevel.Main)
            {
                return _menuItemReturnMain.Shortcut; // Go back to main menu
            }
            
        } while (true);
    }


    private MenuItem DisplayMenuGetUserChoice()
    {
        var userInput = "";

        do
        {
            DrawMenu();
            
            userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("You entered empty string.");
                Console.WriteLine();
            }
            else
            {
                userInput = userInput.ToUpper();
                
                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.Shortcut.ToUpper() != userInput) continue;
                    return menuItem;
                }

                Console.WriteLine("Use existing options.");
                Console.WriteLine();
            }
            
        } while (true);

    }

    private void DrawMenu()
    {
        Console.Write(MenuHeader);
        Console.WriteLine();
        Console.Write(_menuDivider);
        Console.WriteLine();
        
        foreach (var t in MenuItems)
        {
            Console.WriteLine(t);
        }
        
        Console.WriteLine();
        
        Console.Write(">");
    }
}