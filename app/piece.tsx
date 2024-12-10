import React from "react";
import { Image, View } from "react-native";
import { calculatePieceDimensions, PiecePosition, imageWidth, imageHeight } from "./puzzleLogic";

interface PieceProps {
  imageUri: string;
  gridRow: number;
  gridCol: number;
  piecePosition: PiecePosition;
}


const Piece: React.FC<PieceProps> = ({ imageUri, gridRow, gridCol, piecePosition }) => {
  const { pieceWidth, pieceHeight } = calculatePieceDimensions(imageWidth, imageHeight, gridRow, gridCol);

  return (
    <View style={[{ width: pieceWidth, height: pieceHeight, overflow: "hidden" }]}>
      <Image
        source={{ uri: imageUri }}
        style={{
          width: imageWidth,
          height: imageHeight,
          transform: [{ translateX: -piecePosition.col * pieceWidth }, { translateY: -piecePosition.row * pieceHeight }],
        }}
      />
    </View>
  );
};

export default Piece;
