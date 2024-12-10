import React, {useState} from "react";
import { View, Button, StyleSheet, TextInput, Alert } from "react-native";
import { launchImageLibrary } from "react-native-image-picker";
import { useLocalSearchParams } from "expo-router";
import PuzzleSlide from "./puzzleslide";

const ImageSelector: React.FC = () => {
  const { puzzleType } = useLocalSearchParams();
  const [selectedImage, setSelectedImage] = useState<string | null>(null);
  const [isPuzzlefied, setIsPuzzlefied] = useState(false);
  const [gridSize, setGridSize] = useState({ gridRow: 3, gridCol: 3 });

  const handleImageSelection = () => {
    launchImageLibrary({ mediaType: "photo", includeBase64: false }, (response) => {
      if (response.didCancel) {
        console.log("User cancelled image picker");
      } else if (response.errorMessage) {
        console.log("ImagePicker Error: ", response.errorMessage);
      } else if (response.assets && response.assets.length > 0) {
        const uri = response.assets[0].uri;
        setSelectedImage(uri || null);
        setIsPuzzlefied(true);
      }
    });
  };
  return (
    <View style={styles.container}>

      {selectedImage &&
        isPuzzlefied && 
          <PuzzleSlide
            imageUri={selectedImage}
            gridRow={gridSize.gridRow}
            gridCol={gridSize.gridCol}
          />}
        

        
      {!selectedImage && <Button title="Select an image" onPress={handleImageSelection} />}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  input: {
    width: "80%",
    height: 40,
    borderColor: "gray",
    borderWidth: 1,
  },
});

export default ImageSelector;