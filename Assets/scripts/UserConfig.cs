using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class UserConfig : MonoBehaviour
{
    public ProcGenConfig PCGConfig;
    [SerializeField] public TMP_Text seed_text;
    [SerializeField] public TMP_Text projName_text;
    [SerializeField] public TMP_Dropdown height_dropdown;
    [SerializeField] public TMP_Dropdown width_dropdown;
    [SerializeField] public TMP_InputField seed_inptfield = null;
    [SerializeField] public TMP_InputField projName_inputfield = null;
    [SerializeField] public TMP_InputField scale_inptfield = null;
    [SerializeField] public TMP_InputField octaves_inptfield = null;
    [SerializeField] public TMP_InputField lacunarity_inptfield = null;
    [SerializeField] public TMP_InputField sealevel_inptfield = null;
    [SerializeField] public TMP_InputField depth_inptfield = null;
    [SerializeField] public TMP_InputField a_inptfield = null;
    [SerializeField] public TMP_InputField b_inptfield = null;
    [SerializeField] public Toggle usetrueequator_toggle;
    [SerializeField] public Toggle usefalloffmap_toggle;
    [SerializeField] public Toggle randomoffset_toggle;
    [SerializeField] public TMP_InputField tempbias_inptfield = null;
    [SerializeField] public TMP_InputField tempheight_inptfield = null;
    [SerializeField] public TMP_InputField basetemp_inptfield = null;
    [SerializeField] public TMP_InputField spread_inptfield = null;
    [SerializeField] public TMP_InputField spreadthreshold_inptfield = null;
    [SerializeField] public TMP_InputField dewpoint_inptfield = null;
    [SerializeField] public TMP_InputField precintensity_inptfield = null;
    [SerializeField] public Slider persistence_slider = null;
    [SerializeField] public Slider temp_loss_slider = null;
    [SerializeField] public Slider hft_slider = null;


    void Awake()
    {

        seed_text.text = PCGConfig.seed.ToString();

    }

    //Project description

    public void setSeed()
    {

        int seedValue = int.Parse(seed_inptfield.text);
        PCGConfig.seed = seedValue;
        seed_text.text = PCGConfig.seed.ToString();
            
    }

    public void setProjName()
    {

        projName_text.text = projName_inputfield.text.ToString();

    }   

    public void saveChanges()
    {

        setSeed();
        setProjName();

    }

    //Height Map Settings
    public void setHeight(int val)
    {

        if(val == 0)
        {

            PCGConfig.height = 512;

        }

        if(val == 1)
        {

            PCGConfig.height = 256;

        }
        
        if(val == 2)
        {

            PCGConfig.height = 1024;

        }

    }

    public void setWidth(int val)
    {

        if(val == 0)
        {

            PCGConfig.width = 512;

        }

        if(val == 1)
        {

            PCGConfig.width = 256;

        }
        
        if(val == 2)
        {

            PCGConfig.width = 1024;

        }

    }

       public void setScale(float scale)
    {

        float scaleValue = float.Parse(scale_inptfield.text);
        PCGConfig.scale = scaleValue;

    }

     public void setOctaves(int octaves)
    {

        int octavesValue = int.Parse(octaves_inptfield.text);
        PCGConfig.octaves = octavesValue;

    }

    public void setPersistence(float persistance)
    {

        PCGConfig.persistance = persistence_slider.value;

    }

    public void setLacunarity(float lacunarity)
    {

        float lacunarityValue = float.Parse(lacunarity_inptfield.text);
        PCGConfig.lacunarity = lacunarityValue;

    }

    public void setSeaLevel(float seaLevel)
    {

        float seaLevelValue = float.Parse(sealevel_inptfield.text);
        PCGConfig.seaLevel = seaLevelValue;

    }

    public void setDepth(int depth)
    {

        int depthValue = int.Parse(depth_inptfield.text);
        PCGConfig.depth = depthValue;

    }

    //Fall off map variables settings

    public void setA(float a)
    {

        float aValue = float.Parse(a_inptfield.text);
        PCGConfig.a = aValue;

    }

    public void setB(float b)
    {

        float bValue = float.Parse(b_inptfield.text);
        PCGConfig.b = bValue;

    }

    //Booleans settings

    public void setUTEToggle(bool useTrueEquator)
    {

        PCGConfig.useTrueEquator = usetrueequator_toggle.isOn;

    }

     public void setUFMToggle(bool useFalloffMap)
    {

        PCGConfig.useFalloffMap = usefalloffmap_toggle.isOn;

    }

    public void setRandomOffsetToggle(bool randomOffset)
    {

        PCGConfig.randomOffset = randomoffset_toggle.isOn;

    }

    //Temperature map related variable settings

     public void setTempBias(float temperatureBias)
    {

        float tempBiasValue = float.Parse(tempbias_inptfield.text);
        PCGConfig.temperatureBias = tempBiasValue;

    }

    public void setTempHeight(float tempHeight)
    {

        float tempHeightValue = float.Parse(tempheight_inptfield.text);
        PCGConfig.tempHeight = tempHeightValue;

    }

        public void setTempLoss(float octaves)
    {

       PCGConfig.tempLoss = temp_loss_slider.value;

    }

     public void setBaseTemp(float baseTemp)
    {

        float baseTempValue = float.Parse(basetemp_inptfield.text);
        PCGConfig.baseTemp = baseTempValue;

    }

    public void setSpread(float spread)
    {

        float spreadValue = float.Parse(spread_inptfield.text);
        PCGConfig.spread = spreadValue;

    }

    public void setSpreadThreshold(float spreadThreshold)
    {

        float spreadThresholdValue = float.Parse(spreadthreshold_inptfield.text);
        PCGConfig.spreadThreshold = spreadThresholdValue;

    }

    //Humidity/Precipitation Related Variables Settings

    public void setDewPoint(float dewPoint)
    {

        float dewPointValue = float.Parse(dewpoint_inptfield.text);
        PCGConfig.dewPoint = dewPointValue;

    }

    public void setPrecIntensity(float precipitationIntensity)
    {

        float precIntensityValue = float.Parse(precintensity_inptfield.text);
        PCGConfig.precipitationIntensity = precIntensityValue;

    }

        public void setHFT(float humidityFlatteningThreshold)
    {

        PCGConfig.humidityFlatteningThreshold = hft_slider.value;

    }

    public void backToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
