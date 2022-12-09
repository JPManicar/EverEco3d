using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class UserConfig : MonoBehaviour
{
    public ProcGenConfig PCGConfig;
    public GenerationManager genManager;
    [SerializeField] public TMP_Dropdown height_dropdown;
    [SerializeField] public TMP_Dropdown width_dropdown;
    [SerializeField] public Slider scale_slider = null;
    [SerializeField] public Slider octaves_slider= null;
    [SerializeField] public Slider lacunarity_slider = null;
    [SerializeField] public Slider depth_slider = null;
    [SerializeField] public Slider a_slider = null;
    [SerializeField] public Slider b_slider = null;
    [SerializeField] public Toggle usetrueequator_toggle;
    [SerializeField] public Toggle usefalloffmap_toggle;
    [SerializeField] public Toggle randomoffset_toggle;
    [SerializeField] public TMP_Text seed_text;
    [SerializeField] public TMP_Text projName_text;
    [SerializeField] public TMP_InputField seed_inptfield = null;
    [SerializeField] public TMP_InputField projName_inputfield = null;
    [SerializeField] public Slider tempbias_slider = null;
    [SerializeField] public Slider tempheight_slider = null;
    [SerializeField] public Slider basetemp_slider = null;
    [SerializeField] public Slider spread_slider = null;
    [SerializeField] public Slider spreadthreshold_slider = null;
    [SerializeField] public Slider dewpoint_slider = null;
    [SerializeField] public Slider precintensity_slider = null;
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

        PCGConfig.scale = scale_slider.value;

    }

     public void setOctaves(int octaves)
    {

        PCGConfig.octaves = (int)octaves_slider.value;

    }

    public void setPersistence(float persistance)
    {

        PCGConfig.persistance = persistence_slider.value;

    }

    public void setLacunarity(float lacunarity)
    {

        PCGConfig.lacunarity= lacunarity_slider.value;

    }

    public void setDepth(int depth)
    {

        PCGConfig.depth = (int)depth_slider.value;

    }

    //Fall off map variables settings

    public void setA(float a)
    {

        PCGConfig.a= a_slider.value;

    }

    public void setB(float b)
    {

        PCGConfig.b= b_slider.value;

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

        PCGConfig.temperatureBias= tempbias_slider.value;

    }

    public void setTempHeight(float tempHeight)
    {

        PCGConfig.tempHeight= tempheight_slider.value;

    }

        public void setTempLoss(float octaves)
    {

       PCGConfig.tempLoss = temp_loss_slider.value;

    }

     public void setBaseTemp(float baseTemp)
    {

        PCGConfig.baseTemp= basetemp_slider.value;

    }

    public void setSpread(float spread)
    {

        PCGConfig.spread= spread_slider.value;

    }

    public void setSpreadThreshold(float spreadThreshold)
    {

        PCGConfig.spreadThreshold= spreadthreshold_slider.value;

    }

    //Humidity/Precipitation Related Variables Settings

    public void setDewPoint(float dewPoint)
    {

        PCGConfig.dewPoint= dewpoint_slider.value;

    }

    public void setPrecIntensity(float precipitationIntensity)
    {

        PCGConfig.precipitationIntensity= precintensity_slider.value;

    }

        public void setHFT(float humidityFlatteningThreshold)
    {

        PCGConfig.humidityFlatteningThreshold = hft_slider.value;

    }

    public void backToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void resetToDefault()
    {

        PCGConfig.scale = 50;
        scale_slider.value = 50;

        PCGConfig.octaves = 7;
        octaves_slider.value = 7;

        PCGConfig.lacunarity = 2;
        lacunarity_slider.value = 2;

        PCGConfig.persistance = 0.6f;
        persistence_slider.value = 0.6f;

        PCGConfig.depth = 55;
        depth_slider.value = 55;

        PCGConfig.a = 1.3f;
        a_slider.value = 1.3f;

        PCGConfig.b = 5.3f;
        b_slider.value = 5.3f;

        PCGConfig.temperatureBias = 3.3f;
        tempbias_slider.value = 3.3f;

        PCGConfig.tempHeight = 0.11f;
        tempheight_slider.value = 0.11f;

        PCGConfig.baseTemp = 0.5f;
        basetemp_slider.value = 0.5f;

        PCGConfig.tempLoss = 0.161f;
        temp_loss_slider.value = 0.161f;

        PCGConfig.spread = 16;
        spread_slider.value = 16;

        PCGConfig.spreadThreshold = 3;
        spreadthreshold_slider.value = 3;

        PCGConfig.dewPoint = 30;
        dewpoint_slider.value = 30;

        PCGConfig.precipitationIntensity = 0.8f;
        precintensity_slider.value = 0.8f;

        PCGConfig.humidityFlatteningThreshold = 0.4f;
        hft_slider.value = 0.4f;
    }

    public void applyChanges()
    {

        genManager.GenerateWorld();

    }
}
