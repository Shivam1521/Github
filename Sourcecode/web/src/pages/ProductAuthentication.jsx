import React, { useEffect, useState } from "react";
import { Backdrop, Box, CircularProgress, Typography } from "@mui/material";
import Success from "../assets/gifs/Success.gif";
import Alogo from "../assets/AnchorLogo.png";
import logo from "../assets/images/logo.png";
import CopyrightIcon from "@mui/icons-material/Copyright";
import SecurityIcon from "@mui/icons-material/Security";
import PAService from "../services/PAService";
import Error from "../assets/gifs/Error.gif";
import WarningAmberIcon from "@mui/icons-material/GppBad";

const Loader = () => {
  return (
    <Backdrop open className="dashboard-loader">
      <Box
        component="img"
        src={Alogo}
        alt="Loading..."
        className="loader-image"
      />
    </Backdrop>
  );
};
export const ProductAuthentication = () => {
  const paService = new PAService();
  const [loading, setLoading] = useState(false);
  const [details, setDetails] = useState({});
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchQRData = async () => {
      const payload = {
        qrData: window.location.href,
        verificationMode: "Web",
        city: "",
        state: "",
        country: "",
        latitude: "",
        longitude: "",
        qrType: "",
      };

      setLoading(true);

      try {
        const res = await paService.VerifyQR(payload);

        if (res.responseCode === "200") {
          try {
            if (res.responseData == "") {
              setDetails({
                responseMessage: res.responseMessage,
                responseData: [],
              });
            } else {
              const parsedData = JSON.parse(res.responseData);
              setDetails({
                responseMessage: res.responseMessage,
                responseData: parsedData,
              });
            }
          } catch (e) {
            setError(res.responseMessage);
          }
        } else {
          setError(res.responseMessage);
        }
      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    };
    fetchQRData();
  }, []);

  console.log(details);
  return (
    <Box sx={{ minHeight: "100vh", width: "100%" }}>
      {loading && <Loader size={500} />}
      {!loading && (
        <Box
          sx={{
            position: "relative",
            minHeight: "100vh",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            overflow: "hidden",
          }}
        >
          <video
            autoPlay
            loop
            muted
            playsInline
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              width: "100%",
              height: "100%",
              objectFit: "cover",
              zIndex: 0,
            }}
          >
            <source src="/video.mp4" type="video/mp4" />
          </video>
          <Box
            component="img"
            src={logo}
            alt="Panasonic Logo"
            sx={{
              position: "absolute",
              top: 20,
              left: "50%",
              transform: "translateX(-50%)",
              height: { md: 60, xs: 40 },
              zIndex: 2,
            }}
          />

          <Box
            sx={{
              position: "relative",
              zIndex: 1,
              px: { md: 4, xs: 2 },
              py: { md: 1, xs: 1 },
              mt: 3,
              // mx: 3,
              width: { md: 500, xs: 300 },
              borderRadius: "16px",
              background: "rgba(152, 150, 150, 0.24)",
              backdropFilter: "blur(10px)",
              boxShadow: "0 8px 32px rgba(0, 0, 0, 0.3)",
              border: "1px solid rgba(255, 255, 255, 0.2)",
              textAlign: "center",
              animation: "floatUpDown 4s ease-in-out infinite",
              "@keyframes floatUpDown": {
                "0%, 100%": { transform: "translateY(0)" },
                "50%": { transform: "translateY(-10px)" },
              },
            }}
          >
            <Box
              component="img"
              src={error ? Error : Success}
              alt="Authentication Success"
              sx={{
                width: 200,
                height: 200,
                objectFit: "contain",
              }}
            />
            {!error && (
              <>
                <Typography
                  variant="h4"
                  sx={{
                    fontWeight: "bold",
                    mb: 1,
                    background: "linear-gradient(90deg, #00A0E3, #00E5FF)",
                    WebkitBackgroundClip: "text",
                    WebkitTextFillColor: "transparent",
                    textShadow: "0 2px 8px rgba(0, 160, 227, 0.4)",
                  }}
                >
                  {details.responseMessage}
                </Typography>

                {/* <Typography
              variant="body1"
              sx={{
                color: "rgb(255, 255, 255)",
                fontWeight: 600,
                fontSize: "1.2rem",
                textShadow: "0 0 12px rgba(110, 110, 110, 0.8)",
              }}
            >
              You have bought a <strong>Genuine Product</strong> from Panasonic.
            </Typography> */}
                <Box>
                  {details?.responseData == [] && (
                    <Typography
                      sx={{
                        mb: 0.5,
                        mt: 2,
                        fontWeight: 600,
                        fontSize: "16px",
                        textAlign: "left",
                        color: "rgb(210, 183, 5)",
                      }}
                    >
                      Product Details:
                    </Typography>
                  )}

                  {details?.responseData?.map((item, idx) => (
                    <Box
                      key={idx}
                      sx={{
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                        mb: 1,
                        px: 1.5,
                        py: 1,
                        borderRadius: "8px",
                        background: "rgba(255,255,255,0.05)",
                        border: "1px solid rgba(255,255,255,0.15)",
                      }}
                    >
                      <Typography
                        variant="subtitle2"
                        sx={{ fontWeight: 600, color: "#e0e0e0" }}
                      >
                        {item.Key}
                      </Typography>
                      <Typography
                        variant="body2"
                        sx={{
                          color: "#fff",
                          fontWeight: 500,
                          maxWidth: "60%",
                          textAlign: "right",
                        }}
                      >
                        {item.Value}
                      </Typography>
                    </Box>
                  ))}
                </Box>
              </>
            )}
            {error && (
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "center",
                  color: "#fff",
                  textShadow: "0 0 8px rgba(210, 44, 11, 0.6)",
                  alignItems: "center",
                  background: "rgba(212, 59, 8, 0.77)",
                  px: 1.5,
                  py: 0.5,
                  borderRadius: "6px",
                  mx: "auto",
                  width: "fit-content",
                  mb: 1,
                }}
              >
                <WarningAmberIcon sx={{ color: "#fff", mr: 1 }} />
                <Typography
                  sx={{
                    fontWeight: 600,
                    fontSize: "1.2rem",
                  }}
                >
                  {error}
                </Typography>
              </Box>
            )}
          </Box>
          <Box
            component="footer"
            sx={{
              position: "fixed",
              bottom: 0,
              width: "100%",
              py: 1.5,
              px: 1,
              zIndex: 1300,
              display: "flex",
              justifyContent: "space-between",
              background: "linear-gradient(90deg, #1e1e1e, #2c2c2c)",
              borderTop: "1px solid #333",
              color: "#ccc",
              fontSize: "0.9rem",
              letterSpacing: "0.3px",
              transition: "all 0.3s ease",
              boxShadow: "0 -2px 6px rgba(0,0,0,0.3)",
            }}
          >
            <Typography
              variant="body2"
              sx={{
                display: "flex",
                alignItems: "center",
                gap: 0.5,
              }}
            >
              <CopyrightIcon fontSize="small" sx={{ color: "#888" }} />
              {new Date().getFullYear()} Smart Scan Application
            </Typography>
            <Typography
              variant="body2"
              sx={{
                display: "flex",
                alignItems: "center",
                gap: 0.5,
              }}
            >
              <SecurityIcon fontSize="small" />
              Developed by{" "}
              <Box
                component="a"
                href="https://holostik.com/"
                target="_blank"
                sx={{
                  ml: 0.5,

                  color: "#00A0E3",
                  textDecoration: "none",
                  "&:hover": {
                    textDecoration: "underline",
                    color: "#33b5ff",
                  },
                }}
              >
                HOLOSTIK INDIA LTD
              </Box>
            </Typography>
          </Box>
        </Box>
      )}
    </Box>
  );
};
