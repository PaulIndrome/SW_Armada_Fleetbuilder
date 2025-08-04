public static class CurrentDeck {

    private static Deck deck = null;
    public static Deck Deck => deck;

    public static void SetDeck(Deck newDeck){
        if(deck != null && deck != newDeck){
            deck.DestroyDeck();
        }
        deck = newDeck;
    }

}