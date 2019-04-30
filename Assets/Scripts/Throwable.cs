
using UnityEngine;

public sealed class Throwable : MonoBehaviour
{
    public enum Visual
    {
        None,
        Watermelon,
        Book,
        Calculator,
        SoccerBall,
        ToyBear,
        ToyMonkey,
        ToyPenguin,
        ToyPig,
        ToyRabbit,
        ToySheep,
        Shuriken1,
        Shuriken2,
        Shuriken3,
        Shuriken4,
        Shuriken5,
        Shuriken6,
        Bomb,
        Dynamite,
    }

    public GameObject Watermelon;
    public GameObject Book;
    public GameObject Calculator;
    public GameObject SoccerBall;
    public GameObject ToyBear;
    public GameObject ToyMonkey;
    public GameObject ToyPenguin;
    public GameObject ToyPig;
    public GameObject ToyRabbit;
    public GameObject ToySheep;
    public GameObject Shuriken1;
    public GameObject Shuriken2;
    public GameObject Shuriken3;
    public GameObject Shuriken4;
    public GameObject Shuriken5;
    public GameObject Shuriken6;
    public GameObject Bomb;
    public GameObject Dynamite;

    public void SetVisual(Visual visual)
    {
        Watermelon.SetActive(visual == Visual.Watermelon);
        Book.SetActive(visual == Visual.Book);
        Calculator.SetActive(visual == Visual.Calculator);
        SoccerBall.SetActive(visual == Visual.SoccerBall);
        ToyBear.SetActive(visual == Visual.ToyBear);
        ToyMonkey.SetActive(visual == Visual.ToyMonkey);
        ToyPenguin.SetActive(visual == Visual.ToyPenguin);
        ToyPig.SetActive(visual == Visual.ToyPig);
        ToyRabbit.SetActive(visual == Visual.ToyRabbit);
        ToySheep.SetActive(visual == Visual.ToySheep);
        Shuriken1.SetActive(visual == Visual.Shuriken1);
        Shuriken2.SetActive(visual == Visual.Shuriken2);
        Shuriken3.SetActive(visual == Visual.Shuriken3);
        Shuriken4.SetActive(visual == Visual.Shuriken4);
        Shuriken5.SetActive(visual == Visual.Shuriken5);
        Shuriken6.SetActive(visual == Visual.Shuriken6);
        Bomb.SetActive(visual == Visual.Bomb);
        Dynamite.SetActive(visual == Visual.Dynamite);
    }
}
