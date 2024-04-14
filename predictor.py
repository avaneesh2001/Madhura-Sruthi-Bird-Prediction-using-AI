import librosa
import pickle
import numpy as np
import matplotlib.pyplot as plt
from PIL import Image
import os
from keras.preprocessing import image
import numpy as np
import tensorflow as tf
import sys

def save_spectrogram(spectrogram, output_path):
    plt.figure(figsize=(10, 4))
    librosa.display.specshow(librosa.power_to_db(spectrogram, ref=np.max), y_axis='mel', fmax=8000, x_axis='time')
    plt.colorbar(format='%+2.0f dB')
    plt.title('Mel Spectrogram')
    plt.savefig(output_path, bbox_inches='tight', pad_inches=0.1)
    print("Spectoram Saved")
    plt.close()
    
def resize_image(input_path, output_path, size=(224, 224)):
    img = Image.open(input_path)
    img = img.resize(size)
    print("Spectoram Resized")
    img.save(output_path)
    
def CreateSpectrogram(file_path):
    y, sr = librosa.load(file_path, sr=None)    
    spectrogram = librosa.feature.melspectrogram(y=y, sr=sr)
    print("Spectoram MAde")
    return spectrogram


def preprocess_image(image_path):
    img = Image.open(image_path)
    img = img.resize((224, 224))
    img_array = np.array(img)
    img_array = np.expand_dims(img_array, axis=0)
    img_array = img_array / 255.0
    return img_array

try:
    os.makedirs("tmp/")
except FileExistsError:
    print("tmp exists")

filepath = sys.argv[1]
spectrogram = CreateSpectrogram(filepath)
spectrogram_path = os.path.join("tmp", "spectogram") 
save_spectrogram(spectrogram, spectrogram_path)
resize_image(spectrogram_path + ".png", spectrogram_path + ".png", size=(224, 224))

model_path = sys.argv[2]
loaded_model = tf.saved_model.load(f'{model_path}/saved_model')
f = loaded_model.signatures["serving_default"]

image_path = "./tmp/spectogram.png"
preprocessed_image = preprocess_image(image_path)
preprocessed_image = tf.cast(preprocessed_image, tf.float32)
preprocessed_image = preprocessed_image[:, :, :, :3]
prediction_tensors = loaded_model.signatures["serving_default"](tf.constant(preprocessed_image))
predictions = prediction_tensors['output_0'].numpy()[0]
predicted_class = np.argmax(predictions)

with open('labels.pickle', 'rb') as f:
    labels = pickle.load(f)

bird = labels[predicted_class]
print(bird)