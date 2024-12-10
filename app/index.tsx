import { Text, View, Pressable } from "react-native";
import { router, useRouter } from "expo-router";

export default function Index() {
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <Pressable onPress={() => router.push({ pathname: "/imageselector" })}>
        <Text>Puzzle</Text>
      </Pressable>
    </View>
  );
}
