using UnityEngine;

public class CardUnitySquadron : CardUnityBase {
    public SquadronType squadronType;

    public override void SetupCard(Card card, CardTypesLookup cardTypesLookup){
        base.SetupCard(card, cardTypesLookup);
        CardTypesLookupSlotSquadron cardTypesLookupSlotSquadron = cardTypesLookup.squadrons.Find(sqt => sqt.cardTypeRaw == card.cardType);
        if(cardTypesLookupSlotSquadron == null) {
            Debug.LogError($"No internal squadron type found for raw type \"{card.cardType}\". Squadron card not set up completely.");
            return;
        }
        squadronType = cardTypesLookupSlotSquadron.squadronType;
    }

}