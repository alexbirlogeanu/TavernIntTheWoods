using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/*this script is used to update most of the HUD display once per turn. */
public class UpdateDisplayInfo : MonoBehaviour {
    public UnityEngine.UI.Text GoldValueText;
    public UnityEngine.UI.Text WaveValueText;
    public UnityEngine.UI.Text EnemiesValueText;

    //wave info widgets
    public UnityEngine.UI.Text ResistanceValueText;
    public UnityEngine.UI.Text WeaknessValueText;
    public UnityEngine.UI.Text HPValueText;
    // Use this for initialization
    void Start ()
    {
        Assert.IsNotNull(GoldValueText, "UpdateDisplayInfo.Start: GoldValueText is NULL");
        Assert.IsNotNull(WaveValueText, "UpdateDisplayInfo.Start: WaveValueText is NULL");
        Assert.IsNotNull(EnemiesValueText, "UpdateDisplayInfo.Start: EnemiesValueText is NULL");
        Assert.IsNotNull(ResistanceValueText, "UpdateDisplayInfo.Start: ResistanceValueText is NULL");
        Assert.IsNotNull(WeaknessValueText, "UpdateDisplayInfo.Start: WeaknessValueText is NULL");
        Assert.IsNotNull(HPValueText, "UpdateDisplayInfo.Start: HPValueText is NULL");
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        GameInfoUIUpdate();
        WaveInfoUIUpdate();
    }

    private void WaveInfoUIUpdate()
    {
        EnemyStatus status = GameState.CurrentWave.GetWaveEnemyStatus();
        ResistanceValueText.text = Utils.GetUIStringFromElementType(status.Resistance);
        ResistanceValueText.color = Utils.GetUIColorFromElementType(status.Resistance);

        WeaknessValueText.text = Utils.GetUIStringFromElementType(status.Weakness);
        WeaknessValueText.color = Utils.GetUIColorFromElementType(status.Weakness);

        HPValueText.text = status.Health.ToString();
    }

    private void GameInfoUIUpdate()
    {
        GoldValueText.text = GameState.Gold.ToString();

        WaveValueText.text = (GameState.CurrentWaveIndex + 1) + "/" + GameState.MaxWaves;
        EnemiesValueText.text = GameState.EnemiesEscaped + "/" + GameState.EnemiesEscapedTreshold;

        float escapedPercent = (float)GameState.EnemiesEscaped / (float)GameState.EnemiesEscapedTreshold;

        if (escapedPercent <= 0.5f)
            EnemiesValueText.color = Color.green;
        else if (escapedPercent < 0.75f)
            EnemiesValueText.color = Color.yellow;
        else
            EnemiesValueText.color = Color.red;
    }
}
