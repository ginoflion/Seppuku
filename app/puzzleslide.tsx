import React, { useEffect, useState } from "react";
import { Image, View, StyleSheet, Button, Text } from "react-native";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import RNPickerSelect from "react-native-picker-select";
import {
  generateInitialPositions,
  shufflePositions,
  PiecePosition,
  isPuzzleSlideSolved,
  imageHeight,
  imageWidth,
  isSolvable,
  PuzzleState,
} from "./puzzleLogic";
import Piece from "./piece";
import { bfsSolve, aStarSolve } from "./puzzlesolver";

interface PuzzleSlideProps {
  imageUri: string | null;
  gridRow: number;
  gridCol: number;
}

const PuzzleSlide: React.FC<PuzzleSlideProps> = ({ imageUri, gridRow, gridCol }) => {
  const [isPuzzlefied, setIsPuzzlefied] = useState(false);
  const [initialPositions, setInitialPositions] = useState<(PiecePosition | null)[]>([]);
  const [shuffledPieces, setShuffledPieces] = useState<(PiecePosition | null)[]>([]);
  const [emptyIndex, setEmptyIndex] = useState<number>(-1);
  const [solutionPath, setSolutionPath] = useState<(PuzzleState | null)[]>([]);
  const [currentStep, setCurrentStep] = useState(0);
  const [selectedAlgorithm, setSelectedAlgorithm] = useState("aStar");
  const [executionTime, setExecutionTime] = useState("");
  useEffect(() => {
    const positions = generateInitialPositions(gridRow, gridCol).slice(0, gridRow * gridCol - 1);
    positions.push(null);
    setInitialPositions(positions);
    let shuffled = shufflePositions(positions.filter((piece) => piece !== null));
    while (!isSolvable(shuffled, gridCol)) {
      shuffled = shufflePositions(positions.filter((piece) => piece !== null));
    }
    shuffled.push(null);
    setShuffledPieces(shuffled);
    setEmptyIndex(shuffled.length - 1);
  }, [gridRow, gridCol]);

  const handlePuzzleFy = () => {
    setIsPuzzlefied(true);
  };

  const handleReshuffle = () => {
    const positionsWithoutNull = initialPositions.filter((piece) => piece !== null);

    let shuffled = shufflePositions(positionsWithoutNull);

    while (!isSolvable(shuffled, gridCol)) {
      shuffled = shufflePositions(positionsWithoutNull);
    }

    shuffled.push(null);

    setShuffledPieces(shuffled);
    setEmptyIndex(shuffled.length - 1);
    handleManualMove();
  };

  const handleSolve = () => {
    const startTime = Date.now();
    let solution: PuzzleState[] | null = null;
    if (selectedAlgorithm === "bfs") {
      solution = bfsSolve(shuffledPieces as PiecePosition[], initialPositions as PiecePosition[], gridRow, gridCol);
      console.log("bfs");
    } else {
      solution = aStarSolve(shuffledPieces as PiecePosition[], initialPositions as PiecePosition[], gridRow, gridCol);
      console.log("aStar");
    }
    const endTime = Date.now();
    const timeTaken = `${(endTime - startTime).toFixed(2)} ms`;
    setExecutionTime(timeTaken);

    if (solution) {
      setSolutionPath(solution);
    }
  };

  const handleNextStep = () => {
    if (solutionPath && currentStep < solutionPath.length) {
      const nextMove = solutionPath[currentStep];
      setShuffledPieces(nextMove as PuzzleState);
      setCurrentStep(currentStep + 1);
      const emptyIndex = nextMove!.findIndex((piece) => piece === null);
      if (emptyIndex !== -1) {
        setEmptyIndex(emptyIndex);
      }
    }
  };

  const handleManualMove = () => {
    setSolutionPath([]);
    setCurrentStep(0);
  };

  const handleWin = () => {
    return isPuzzleSlideSolved(shuffledPieces as PiecePosition[], initialPositions as PiecePosition[]);
  };

  const pieces = [];

  for (let index = 0; index < shuffledPieces.length; index++) {
    if (shuffledPieces[index] !== null) {
      pieces.push(<Piece imageUri={imageUri!} gridRow={gridRow} gridCol={gridCol} piecePosition={shuffledPieces[index]!} />);
    } else {
      pieces.push(
        <View
          key={index}
          style={[{ width: imageWidth / gridCol, height: imageHeight / gridRow, backgroundColor: "lightgray" }]}
        />
      );
    }
  }

  return (
    <GestureHandlerRootView style={styles.container}>
      <RNPickerSelect
        onValueChange={(value) => setSelectedAlgorithm(value)}
        items={[
          { label: "BFS", value: "bfs" },
          { label: "A*", value: "aStar" },
        ]}
        placeholder={{ label: "Select Algorithm", value: null }}
        value={selectedAlgorithm}
      />
      <View>
        <Text>Execution Times:</Text>
        {selectedAlgorithm === "bfs" ? (
          <Text>BFS: {executionTime}</Text>
        ) : selectedAlgorithm === "aStar" ? (
          <Text>A*: {executionTime}</Text>
        ) : (
          <Text>Please select an algorithm</Text>
        )}
      </View>
      <View>
        <Text>Current Step: {currentStep}</Text>
      </View>
      {isPuzzlefied ? (
        <View
          style={[
            styles.puzzleContainer,
            { width: imageWidth, height: imageHeight, overflow: "hidden", borderRadius: imageWidth / gridCol / 10 },
          ]}
        >
          {pieces}
        </View>
      ) : (
        <Image
          source={{ uri: imageUri! }}
          style={{ width: imageWidth, height: imageHeight, overflow: "hidden", borderRadius: imageWidth / gridCol / 10 }}
        />
      )}
      {!isPuzzlefied && <Button title="Puzzlefy" onPress={handlePuzzleFy} />}
      {isPuzzlefied && <Button title="Reshuffle" onPress={handleReshuffle} />}
      {isPuzzlefied && <Button title="Solve" onPress={handleSolve} />}
      {isPuzzlefied && <Button title="Next Step" onPress={handleNextStep} />}
      {handleWin() && <Text>You win!</Text>}
    </GestureHandlerRootView>
  );
};

const styles = StyleSheet.create({
  container: {
    alignItems: "center",
    maxHeight: "100%",
  },
  puzzleContainer: {
    flexDirection: "row",
    flexWrap: "wrap",
  },
});

export default PuzzleSlide;
