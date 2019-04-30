
using UnityEngine;

public sealed class Consumable : MonoBehaviour
{
    public enum Visual
    {
        None,
        LoveCandies,
        TinnedFood1,
        TinnedFood2,
        TinnedFood3,
        Cake,
        Donut,
        Hamburger,
        Apple,
        Pear,
        Kiwi,
    }

    public GameObject LoveCandies;
    public GameObject TinnedFood1;
    public GameObject TinnedFood2;
    public GameObject TinnedFood3;
    public GameObject Cake;
    public GameObject Donut;
    public GameObject Hamburger;
    public GameObject Apple;
    public GameObject Pear;
    public GameObject Kiwi;

    public void SetVisual(Visual visual)
    {
        LoveCandies.SetActive(visual == Visual.LoveCandies);
        TinnedFood1.SetActive(visual == Visual.TinnedFood1);
        TinnedFood2.SetActive(visual == Visual.TinnedFood2);
        TinnedFood3.SetActive(visual == Visual.TinnedFood3);
        Cake.SetActive(visual == Visual.Cake);
        Donut.SetActive(visual == Visual.Donut);
        Hamburger.SetActive(visual == Visual.Hamburger);
        Apple.SetActive(visual == Visual.Apple);
        Pear.SetActive(visual == Visual.Pear);
        Kiwi.SetActive(visual == Visual.Kiwi);
    }
}
