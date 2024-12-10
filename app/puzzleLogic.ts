//Define a estrutura de dados para representar a posição de uma peça
//Cada peça é identificada pela fila e coluna onde se encontra
export interface PiecePosition {
    row: number;
    col: number;
  }
  
  export interface GridCell extends PiecePosition {
    isOccupied: boolean;
  }
    
    
  
  
  export type PuzzleState = PiecePosition[];
  
  export const imageWidth = 500;
  export const imageHeight = 300;
  //Função para calcular as dimensões de cada peça dependendo da grid
  export const calculatePieceDimensions = (
    imageWidth: number,
    imageHeight: number,
    gridRow: number,
    gridCol: number
  ): { pieceWidth: number; pieceHeight: number } => {
    return {
      pieceWidth: imageWidth / gridCol,
      pieceHeight: imageHeight / gridRow,
    };
  };
  
  //Função para gerar as posições iniciais das peças
  export const generateInitialPositions = (gridRow: number, gridCol: number): PiecePosition[] => {
    const positions: PiecePosition[] = [];
    for (let row = 0; row < gridRow; row++) {
      for (let col = 0; col < gridCol; col++) {
        positions.push({ row, col });
      }
    }
    return positions;
  };
  
  //Função para baralhar as posições das peças usando o algoritmo Fisher-Yates
  export const shufflePositions = (positions: PiecePosition[]): PiecePosition[] => {
    const shuffledPositions = [...positions];
    for (let i = shuffledPositions.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [shuffledPositions[i], shuffledPositions[j]] = [shuffledPositions[j], shuffledPositions[i]];
    }
    return shuffledPositions;
  };
  
  //Função para verificar se duas peças são adjacentes
  export const isAdjacent = (index1: number, index2: number, gridCol: number) => {
    const row1 = Math.floor(index1 / gridCol);
    const col1 = index1 % gridCol;
    const row2 = Math.floor(index2 / gridCol);
    const col2 = index2 % gridCol;
  
    return (Math.abs(row1 - row2) === 1 && col1 === col2) || (Math.abs(col1 - col2) === 1 && row1 === row2);
  };
  
  export const isSolved = (initialState: PuzzleState, targetState: PuzzleState): boolean => {
    return JSON.stringify(initialState) === JSON.stringify(targetState);
  };
  
  // Função para verificar se o sliding puzzle foi resolvido
  export const isPuzzleSlideSolved = (
    shuffledPieces: (PiecePosition | null)[],
    initialPositions: (PiecePosition | null)[]
  ): boolean => {
    
    return shuffledPieces.every((piece, index) => {
      if (piece === null || initialPositions[index] === null) {
        return piece === initialPositions[index];
      }
      return piece.row === initialPositions[index]?.row && piece.col === initialPositions[index]?.col;
    });
  };
  export const checkIfPuzzleSolved = (
    shuffledPieces: (PiecePosition | null)[],
    initialPositions: PiecePosition[]
  ): boolean => {
    // Verifique se todas as peças estão nas posições iniciais
    return shuffledPieces.every((piece, index) => {
      if (piece === null) return true; // A posição vazia não precisa ser verificada
      return piece.row === initialPositions[index].row && piece.col === initialPositions[index].col;
    });
  };
  
  export const isSolvable = (puzzle: (PiecePosition | null)[], gridCol: number) => {
    const flatPuzzle = puzzle.filter((piece) => piece !== null);
    let inversions = 0;
  
    for (let i = 0; i < flatPuzzle.length; i++) {
      for (let j = i + 1; j < flatPuzzle.length; j++) {
        if (
          flatPuzzle[i]!.row > flatPuzzle[j]!.row ||
          (flatPuzzle[i]!.row === flatPuzzle[j]!.row && flatPuzzle[i]!.col > flatPuzzle[j]!.col)
        ) {
          inversions++;
        }
      }
    }
    return inversions % 2 === 0;
  };