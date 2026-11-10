import React from "react";
import { Box } from "@mui/material";
import bgImage from "../assets/images/bgImage.png";
import logo from "../assets/images/bg.png";

export default function DBoard({}) {
  return (
    <Box
      sx={{
        position: "relative",
        backgroundImage: `url(${logo})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
        height: "85%",
        width: "100%",
        display: "flex",
        alignItems: "center",
        justifyContent: "flex-start",
        backdropFilter: "blur(4px)",
        px: 3,
        objectFit: "contain"
      }}
    ></Box>
  );
}
