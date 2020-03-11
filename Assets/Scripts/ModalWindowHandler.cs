using System;

public static class ModalWindowHandler {

    private static ModalWindow currentRegisteredModalWindow;
    
    public static void RegisterModalWindow(ModalWindow modalWindow){
        currentRegisteredModalWindow = modalWindow;
    }

    public static void ShowModalWindow(Action<ModalResult> resultHandlingMethod, string title, string description = "", params ModalResult[] possibleResults){
        currentRegisteredModalWindow.ShowModalWindow(resultHandlingMethod, title, description, possibleResults);
    }


}