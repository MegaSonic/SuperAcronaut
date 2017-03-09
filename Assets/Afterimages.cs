using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Afterimages : MonoBehaviour {

    
    public enum ImageState
    {
        OFF = 0,
        ON = 1,
        FADING = 2
    }

    #region Inspector tweakables

    // The number of afterimages to display
    public int imagesToDisplay = 3;

    // The color of the afterimages
    public Color imageColor = Color.red;

    public bool saveSprites = false;

    // How much of a delay should there be between afterimages? 1 for least amount of delay.
    public int delayBetweenImages = 1;

    // How many past positions to save. Adjust based on performance. I don't recommend changing this at runtime.
    public int positionsToSave = 100;

    public bool useMulticolors = false;
    public Color[] multicolors = {Color.red, Color.red, Color.red};
    #endregion

    #region Private members
    private int imagesToDisplayCopy;
    private int delayBetweenImagesCopy;

    private ImageState state;
    private GameObject afterImage;
    private Vector3[] imagePositions;
    private Sprite[] savedSprites;
    private int index = 0;
    private int imagesInPlay = 0;
    private List<Transform> images;
    private SpriteRenderer sprite;

    private GameObject parent;
    #endregion


    // Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;

        imagePositions = new Vector3[positionsToSave];
        savedSprites = new Sprite[positionsToSave];
        images = new List<Transform>(imagesToDisplay);
        sprite = GetComponent<SpriteRenderer>();

        parent = new GameObject("Afterimages");
        parent.transform.position = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < imagesToDisplay; i++)
        {
            afterImage = new GameObject("_afterimage");
            Afterimage image = afterImage.AddComponent<Afterimage>();
            SpriteRenderer imageSprite = afterImage.AddComponent<SpriteRenderer>();
            images.Add(afterImage.transform);

            if (!useMulticolors)
            {

                imageSprite.color = imageColor;
            }
            else
            {
                imageSprite.color = multicolors[i];
            }
            afterImage.gameObject.SetActive(false);
            image.imageInChain = i + 1;
            image.maxOffset = image.imageInChain * delayBetweenImages;
            imageSprite.sortingOrder = sprite.sortingOrder - i - 1;
            afterImage.transform.SetParent(parent.transform, true);
        }

        imagesToDisplayCopy = imagesToDisplay;
        delayBetweenImagesCopy = delayBetweenImages;
	}
	
	// Update is called once per frame
	void Update () {
        CheckForUpdate();

        switch (state) {

            case ImageState.ON:
                {
                    imagePositions[index] = this.gameObject.transform.position;
                    savedSprites[index] = sprite.sprite;

                    foreach (Transform t in images)
                    {
                        if (t.gameObject.activeSelf == false)
                        {
                            t.gameObject.SetActive(true);
                        }

                        Afterimage image = t.GetComponent<Afterimage>();
                        t.position = this.gameObject.transform.position;

                        // If this value is null, it must have started generating images and not looped around yet.
                        if (imagePositions[positionsToSave - 1] == Vector3.zero)
                        {
                            int num = index - image.offset;
                            if (num < 0) num = 0;
                            t.position = imagePositions[num];
                        }
                        else
                        {
                            t.position = imagePositions[SubtractAndWrap(index, image.offset, positionsToSave)];
                        }

                        t.localScale = transform.localScale;
                        SpriteRenderer imageSprite = t.GetComponent<SpriteRenderer>();
                        if (!saveSprites)
                        {
                            imageSprite.sprite = sprite.sprite;
                        }
                        else
                        {
                            imageSprite.sprite = savedSprites[SubtractAndWrap(index, image.offset, positionsToSave)];
                        }

                        if (image.offset < image.maxOffset) image.offset++;
                    }

                    index++;
                    if (index >= positionsToSave) index = 0;
                }
                break;

            case ImageState.OFF:
                break;

            case ImageState.FADING:
            {
                imagePositions[index] = this.gameObject.transform.position;

                foreach (Transform t in images)
                {
                    Afterimage image = t.GetComponent<Afterimage>();

                    // If this value is null, it must have started generating images.
                    if (imagePositions[positionsToSave - 1] == Vector3.zero)
                    {
                        int num = index - image.offset;
                        if (num < 0) num = 0;
                        t.position = imagePositions[num];
                    }
                    else
                    {
                        t.position = imagePositions[SubtractAndWrap(index, image.offset, positionsToSave)];
                    }

                    t.localScale = transform.localScale;

                    if (!saveSprites)
                    {
                        t.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
                    }
                    else
                    {
                        t.GetComponent<SpriteRenderer>().sprite = savedSprites[SubtractAndWrap(index, image.offset, positionsToSave)];
                    }

                    if (image.offset > 0) image.offset--;

                    if (image.imageInChain == imagesToDisplay && image.offset == 0)
                    {
                        disableImages();
                        state = ImageState.OFF;
                        break;
                    }
                }

                index++;
                if (index >= positionsToSave) index = 0;
            }
            break;
        }
	}

    /// <summary>
    /// Starts up the after images. Does nothing if images are currently on.
    /// </summary>
    public void StartImages()
    {
        if (state == ImageState.ON) return;

        imagePositions = new Vector3[positionsToSave];
        imagesInPlay = 0;
        index = 0;
        foreach (Transform t in images)
        {
            if (t.gameObject != null)
            {
                t.GetComponent<Afterimage>().offset = 0;
            }
        }
        state = ImageState.ON;
    }

    /// <summary>
    /// Forces the images to restart, making them retrace themselves from the game object.
    /// </summary>
    public void ForceStartImages()
    {
        imagePositions = new Vector3[positionsToSave];
        imagesInPlay = 0;
        state = ImageState.ON;
        index = 0;
        foreach (Transform t in images)
        {
            t.gameObject.SetActive(true);
            t.GetComponent<Afterimage>().offset = 0;
        }
    }

    /// <summary>
    /// Sets the afterimages to stop. Afterimages will retract in towards the main body.
    /// </summary>
    public void StopImages()
    {
        if (state == ImageState.ON) state = ImageState.FADING;
    }

    /// <summary>
    /// Forces the afterimages to immediately stop and disappear.
    /// </summary>
    public void ForceStopImages()
    {
        disableImages();
        state = ImageState.OFF;
    }

    private void disableImages()
    {
        foreach (Transform t in images)
        {
            t.gameObject.SetActive(false);
        }
    }

    private void CheckForUpdate()
    {
        if (imagesToDisplayCopy != imagesToDisplay)
        {
            foreach (Transform t in images)
            {
                Destroy(t.gameObject);
                // t.gameObject.SetActive(true);
                
            }
            images = new List<Transform>(imagesToDisplay);

            for (int i = 0; i < imagesToDisplay; i++)
            {
                afterImage = new GameObject("_afterimage");
                Afterimage image = afterImage.AddComponent<Afterimage>();
                SpriteRenderer imageSprite = afterImage.AddComponent<SpriteRenderer>();
                images.Add(afterImage.transform);

                imageSprite.color = imageColor;

                if (state == ImageState.OFF)
                {
                    afterImage.gameObject.SetActive(false);
                }
                image.imageInChain = i + 1;
                image.maxOffset = image.imageInChain * delayBetweenImages;
                imageSprite.sortingOrder = sprite.sortingOrder - i - 1;
                afterImage.transform.SetParent(parent.transform, true);
            }
            imagesToDisplayCopy = imagesToDisplay;
        }
        if (delayBetweenImagesCopy != delayBetweenImages)
        {
            for (int i = 0; i < images.Count; i++)
            {
                Afterimage image = images[i].gameObject.GetComponent<Afterimage>();
                image.maxOffset = image.imageInChain * delayBetweenImages;
            }
            delayBetweenImagesCopy = delayBetweenImages;
        }
    }

    private int SubtractAndWrap(int value, int subtract, int wrapAmount)
    {
        if (value - subtract < 0)
        {
            return wrapAmount - (-1 * (value - subtract));
        }
        return value - subtract;
    }

    private class Afterimage : MonoBehaviour
    {
        public int offset = 0;
        public int maxOffset;

        public int imageInChain;
    }

}
