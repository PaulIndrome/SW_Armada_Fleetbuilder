using System;
using System.Collections.Generic;

public static class ModalWindowHandler {

    private static int currentModalWindow = 0;
    private static List<ModalWindow> registeredModalWindows;
    
    public static void RegisterModalWindow(ModalWindow modalWindow){
        if(registeredModalWindows == null) registeredModalWindows = new List<ModalWindow>();
        
        if(registeredModalWindows.Contains(modalWindow)) return;
        registeredModalWindows.Add(modalWindow);
    }

    public static void ShowModalWindow(Action<ModalResult> resultHandlingMethod, string title, string description = "", params ModalResult[] possibleResults){
        registeredModalWindows[currentModalWindow].ShowModalWindow(resultHandlingMethod, title, description, possibleResults);
        currentModalWindow = (currentModalWindow + 1 + registeredModalWindows.Count) % registeredModalWindows.Count;
    }

    public static void ShowModalWindowActions(Action[] resultCallBacks, string title, string description = "", params ModalResult[] possibleResults){
        registeredModalWindows[currentModalWindow].ShowModalWindowActions(resultCallBacks, title, description, possibleResults);
        currentModalWindow = (currentModalWindow + 1 + registeredModalWindows.Count) % registeredModalWindows.Count;
    }

    public static void ShowModalWindowAction(Action resultCallBack, string title, string description = "", params ModalResult[] possibleResults){
        registeredModalWindows[currentModalWindow].ShowModalWindowAction(resultCallBack, title, description, possibleResults);
        currentModalWindow = (currentModalWindow + 1 + registeredModalWindows.Count) % registeredModalWindows.Count;
    }


}