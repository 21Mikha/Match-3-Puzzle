using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public GameObject slotPrefab;  
    public GameObject itemPrefab;
    public PopUpSystem popUpSystem;
    public MatchFeedback matchFeedback;
    public LevelTimer levelTimer;

    public override void InstallBindings()
    {

        Container.Bind<LevelDataLoader>().AsSingle();

        // Bind (prefabs)
        Container.Bind<GameObject>().WithId("SlotPrefab").FromInstance(slotPrefab);
        Container.Bind<GameObject>().WithId("ItemPrefab").FromInstance(itemPrefab);


        Container.Bind<PopUpSystem>().FromInstance(popUpSystem).AsSingle();
        Container.Bind<MatchFeedback>().FromInstance(matchFeedback).AsSingle();
        Container.Bind<LevelTimer>().FromInstance(levelTimer).AsSingle();
    }
}
