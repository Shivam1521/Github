import React, { useState, useEffect } from "react";
import {
  Box,
  Grid,
  Typography,
  ToggleButtonGroup,
  ToggleButton,
  Card,
  CardContent,
  FormControl,
  Select,
  MenuItem,
  InputLabel,
  Paper,
  Avatar,
  TextField,
  Backdrop,
  CircularProgress,
} from "@mui/material";
import {
  Today,
  CalendarMonth,
  Event,
  CheckCircle,
  Pending,
  PrecisionManufacturing,
  Print,
  ReceiptLong,
  Warehouse,
  CalendarToday,
  SwapHoriz,
  Factory,
} from "@mui/icons-material";
import Highcharts, { offset } from "highcharts";
import Highcharts3D from "highcharts/highcharts-3d";
import HighchartsMore from "highcharts/highcharts-more";
import HighchartsReact from "highcharts-react-official";
import SolidGauge from "highcharts/modules/solid-gauge";
import Accessibility from "highcharts/modules/accessibility";
import { useDispatch, useSelector } from "react-redux";
import { fetchUnits } from "../store/commonSlice";
import { fetchDashboardData } from "../store/dashboardSlice";
import logo from "../assets/AnchorLogo.png";
import unitAccessHook from "../components/UnitAccessHook";

// Initialize Highcharts modules
if (typeof HighchartsMore === "function") HighchartsMore(Highcharts);
if (typeof Highcharts3D === "function") Highcharts3D(Highcharts);
if (typeof Highcharts3D === "function") SolidGauge(Highcharts);
if (typeof Highcharts3D === "function") Accessibility(Highcharts);
export const Dashboard = () => {
  const { isSuperAdmin } = unitAccessHook();
  const [selectedRange, setSelectedRange] = useState("week");
  const globalUnit = useSelector((state) => state.unit.selectedUnit);
  const [to, setTo] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [selectedUnit, setSelectedUnit] = useState(globalUnit.toString());
  const { units } = useSelector((state) => state.lists);
  const { data, loading } = useSelector((state) => state.dashboard);

  const dispatch = useDispatch();
  const {
    qcStatusBreakdown,
    jobDetails,
    pickSlip,
    qrPrinting,
    dispatchReceiving,
  } = data;

  useEffect(() => {
    dispatch(fetchUnits());
  }, []);

  useEffect(() => {
    const storedToken = sessionStorage.getItem("token");
    if (storedToken) {
      const newRequest = {
        unitId: selectedUnit,
        toUnitId: to == "" ? 0 : to,
        fromDate,
        toDate,
      };
      dispatch(fetchDashboardData(newRequest));
    } else {
      navigate("/login");
    }
  }, [selectedUnit, to, fromDate, toDate]);

  useEffect(() => {
    setSelectedUnit(globalUnit);
  }, [globalUnit]);

  const gradientMap = {
    Open: "url(#gradient-blue)",
    Completed: "url(#gradient-green)",
    Hold: "url(#gradient-yellow)",
    Accepted: "url(#gradient-purple)",
    Rejected: "url(#gradient-red)",
  };
  const outerSize = 100;
  const innerGap = 2;
  const ringThickness = 12;
  const series = qcStatusBreakdown?.map((item, index) => {
    const outer = outerSize - index * (ringThickness + innerGap);
    const inner = outer - ringThickness;
    return {
      name: item.label,
      size: `${outer}%`,
      innerSize: `${inner}%`,
      zIndex: qcStatusBreakdown.length - index,
      data: [
        {
          name: item.label,
          y: item.value,
          color: gradientMap[item.label],
          showInLegend: true,
        },
        {
          name: "",
          y: 100 - item.value,
          color: "transparent",
          showInLegend: false,
        },
      ],
    };
  });

  const gaugeOptions = {
    chart: {
      type: "pie",
      backgroundColor: "transparent",
      // margin: 20,
      style: { fontFamily: "Poppins, sans-serif" },
      options3d: { enabled: true },
      events: {
        render() {
          const chart = this;
          const defs = chart.renderer.defs;
          if (defs && !chart.renderer.gradientsCreated) {
            const makeGradient = (id, start, end) => {
              const grad = chart.renderer
                .createElement("linearGradient")
                .attr({ id, x1: 0, y1: 0, x2: 1, y2: 1 })
                .add(defs);
              chart.renderer
                .createElement("stop")
                .attr({ offset: 0, "stop-color": start })
                .add(grad);
              chart.renderer
                .createElement("stop")
                .attr({ offset: 1, "stop-color": end })
                .add(grad);
            };

            makeGradient("gradient-blue", "#60a5fa", "#2563eb");
            makeGradient("gradient-green", "#6ee7b7", "#059669");
            makeGradient("gradient-yellow", "#fde68a", "#d97706");
            makeGradient("gradient-purple", "#c084fc", "#7e22ce");
            makeGradient("gradient-red", "#fca5a5", "#dc2626");

            chart.renderer.gradientsCreated = true;
          }
        },
      },
    },

    tooltip: {
      backgroundColor: "rgba(255,255,255,0.95)",
      borderRadius: 8,
      shadow: false,
      useHTML: true,
      pointFormat: `
        <div style="text-align:center;">
          <b style="font-size:14px;color:#111827;">{point.name}</b><br>
          <span style="font-size:18px;font-weight:700;color:{point.color}">{point.y}%</span>
        </div>
      `,
    },

    legend: {
      align: "center",
      verticalAlign: "bottom",
      layout: "horizontal",
      itemDistance: 15,
      symbolRadius: 6,
      y: 0,
      symbolHeight: 14,
      symbolWidth: 14,
      itemStyle: {
        color: "#334155",
        fontWeight: 600,
        fontSize: "13px",
        textTransform: "capitalize",
      },
      itemHoverStyle: { color: "#111827" },
    },

    plotOptions: {
      pie: {
        allowPointSelect: false,
        cursor: "pointer",
        borderWidth: 0,
        dataLabels: { enabled: false },
        showInLegend: true,
        shadow: {
          color: "rgba(0,0,0,0.15)",
          offsetX: 0,
          offsetY: 2,
          opacity: 0.5,
        },
        states: {
          hover: {
            brightness: 0, // stop brightening
            halo: {
              size: 8,
              attributes: {
                "stroke-width": 0,
                stroke: "rgba(0,0,0,0.15)",
              },
            },
          },
        },
        point: {
          events: {
            mouseOver: function () {
              // Disable hover for transparent part
              if (this.color === "transparent") {
                this.series.chart.tooltip.hide();
              }
            },
          },
        },
      },
    },

    series,
    title: { text: null },

    accessibility: { enabled: false },
    credits: { enabled: false },
  };

  const StatCard = ({ icon, title, value, gradientClass }) => (
    <div className={`stat-card ${gradientClass}`}>
      <Avatar className="stat-avatar">{icon}</Avatar>
      <Box className="stat-content">
        <Typography variant="subtitle2" className="stat-title">
          {title}
        </Typography>
        <Typography variant="h5" className="stat-value">
          {value.toLocaleString()}
        </Typography>
      </Box>
    </div>
  );
  const categories = dispatchReceiving?.receivedData?.map((d) => d.label);
  const dispatchVals = dispatchReceiving?.receivedData?.map(
    (d) => d.dispatch ?? null,
  );
  const receivedVals = dispatchReceiving?.receivedData?.map(
    (d) => d.receive ?? null,
  );
  const areaOptions = (from, to) => ({
    chart: {
      type: "area",
      backgroundColor: "transparent",
      margin: [10, -40, 100, 0],
      spacing: [0, 0, 0, 0],
      options3d: {
        enabled: true,
        alpha: 12,
        beta: 18,
        depth: 150,
        viewDistance: 30,
      },
      style: { fontFamily: "Poppins, sans-serif" },
      events: {
        render() {
          const chart = this;
          if (chart.seriesGroup) {
            chart.seriesGroup.translate(0, -5);
          }
        },
      },
    },

    title: { text: null },

    xAxis: [
      {
        categories: categories,
        tickmarkPlacement: "on",
        lineColor: "#cbd5e1",
        tickColor: "#cbd5e1",
        tickWidth: 1,
        labels: {
          enabled: true,
          style: { color: "#374151", fontWeight: 500, fontSize: "12px" },
          y: 10,
        },
        title: {
          text: "Days",
          style: { color: "#444", fontWeight: 600, fontSize: "12px" },
          y: 20,
        },
        gridLineWidth: 0,
        minorGridLineWidth: 0,
      },
      { visible: false },
    ],
    yAxis: {
      title: {
        text: "Quantity",
        margin: 20,
        style: { fontWeight: 600, color: "#444", fontSize: "12px" },
      },
      lineColor: "#cbd5e1",
      lineWidth: 1,
      labels: {
        style: { color: "#374151", fontWeight: 500, fontSize: "11px" },
      },
      gridLineWidth: 0,
    },

    tooltip: {
      shared: true,
      backgroundColor: "rgba(255,255,255,0.95)",
      borderColor: "#ddd",
      borderRadius: 8,
      useHTML: true,
      headerFormat: "<b>{point.category}</b><br/>", // ✅ fixed
      pointFormat: `<span style="color:{series.color}">{series.name}</span>: <b>{point.y} Units</b><br/>`,
    },

    plotOptions: {
      area: {
        depth: 100,
        marker: { enabled: false },
        states: { inactive: { enabled: false } },
        fillOpacity: 1,
        threshold: null,
        connectNulls: true,
        lineWidth: 5,
        lineColor: "rgba(0,0,0,0.15)",
        borderWidth: 0,
        edgeColor: "transparent",
      },
    },

    legend: {
      align: "center",
      verticalAlign: "bottom",
      itemStyle: { fontWeight: "600", color: "#1e293b" },
      itemHoverStyle: { color: "#000" },
    },

    credits: { enabled: false },

    series: [
      {
        name: `Dispatch - ${from}`,
        data: dispatchVals, // ✅ ensure category binding
        color: {
          linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
          stops: [
            [0, "rgba(59,130,246,0.95)"],
            [1, "rgba(59,130,246,0.3)"],
          ],
        },
        zIndex: 2,
      },
      {
        name: `Receiving - ${to}`,
        data: receivedVals,
        color: {
          linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
          stops: [
            [0, "rgba(239,68,68,0.95)"],
            [1, "rgba(239,68,68,0.3)"],
          ],
        },
        zIndex: 1,
      },
    ],
  });
  const handleFilter = (filter) => {
    const today = new Date();
    let from, to;
    to = today.toISOString().split("T")[0];
    if (filter === "today") {
      from = to;
    } else if (filter === "week") {
      const dayOfWeek = today.getDay();
      const diffToMonday = dayOfWeek === 0 ? 6 : dayOfWeek - 1;
      const monday = new Date(today);
      monday.setDate(today.getDate() - diffToMonday);
      from = monday.toISOString().split("T")[0];
    } else if (filter === "month") {
      const monthStart = new Date(today.getFullYear(), today.getMonth(), 2);
      from = monthStart.toISOString().split("T")[0];
    } else if (filter === "year") {
      const yearStart = new Date(today.getFullYear(), 0, 2);
      from = yearStart.toISOString().split("T")[0];
    }
    setFromDate(from);
    setToDate(to);
  };
  const getUnitLabel = (value) => {
    const unit = units?.find((u) => u.value === value);
    return unit ? unit.label : "";
  };
  useEffect(() => {
    handleFilter(selectedRange);
  }, [selectedRange]);
  return (
    <>
      {selectedUnit ? (
        <Box className="dashboard-wrapper">
          {/* Header */}
          <Box className="dashboard-header new-style">
            <Box className="header-left ">
              <Box className="d-flex align-items-center">
                <Typography>Select Unit</Typography>
                <FormControl
                  size="small"
                  className="unit-select"
                  disabled={!isSuperAdmin}
                >
                  {units.length > 0 && (
                    <Select
                      value={selectedUnit || ""}
                      onChange={(e) => setSelectedUnit(e.target.value)}
                      renderValue={(selected) => {
                        
                        const unit = units.find(
                          (u) => u.value === selected.toString(),
                        );
                        return (
                          <Box display="flex" alignItems="center" gap={1.5}>
                            <Factory
                              sx={{ color: "rgb(255, 12, 12)", fontSize: 20 }}
                            />
                            <span>{unit?.label}</span>
                          </Box>
                        );
                      }}
                    >
                      {units.map((unit) => (
                        <MenuItem key={unit.value} value={unit.value}>
                          <Box display="flex" alignItems="center" gap={1.5}>
                            <Factory sx={{ color: "#475569", fontSize: 18 }} />
                            {unit.label}
                          </Box>
                        </MenuItem>
                      ))}
                    </Select>
                  )}
                </FormControl>
              </Box>
            </Box>

            {/* Middle Section — Date Range */}
            <Box className="header-center">
              <Box className="date-wrapper">
                <Typography className="date-title">From Date</Typography>
                <TextField
                  type="date"
                  size="small"
                  value={fromDate}
                  onChange={(e) => setFromDate(e.target.value)}
                  inputProps={{
                    max: toDate || new Date().toISOString().split("T")[0],
                  }}
                />
              </Box>

              <SwapHoriz className="swap-icon" sx={{ marginBottom: "8px" }} />

              <Box className="date-wrapper">
                <Typography className="date-title">To Date</Typography>

                <TextField
                  type="date"
                  size="small"
                  value={toDate}
                  inputProps={{
                    min: fromDate,
                  }}
                  onChange={(e) => setToDate(e.target.value)}
                />
              </Box>
            </Box>

            {/* Right Section — Toggle Group */}
            <Box className="header-right">
              <ToggleButtonGroup
                value={selectedRange}
                exclusive
                onChange={(e, value) => {
                  if (value) {
                    setSelectedRange(value);
                    handleFilter(value);
                  }
                }}
                className="toggle-group"
              >
                <ToggleButton value="today">
                  <Today className="toggle-icon" /> Today
                </ToggleButton>
                <ToggleButton value="week">
                  <CalendarMonth className="toggle-icon" /> Week
                </ToggleButton>
                <ToggleButton value="month">
                  <CalendarMonth className="toggle-icon" /> Month
                </ToggleButton>
                <ToggleButton value="year">
                  <Event className="toggle-icon" /> Year
                </ToggleButton>
              </ToggleButtonGroup>
            </Box>
          </Box>
          {loading ? (
            <Backdrop open className="dashboard-loader">
              <Box
                component="img"
                src={logo}
                alt="Loading..."
                className="loader-image"
              />
            </Backdrop>
          ) : (
            <>
              <Box className="dashboard-stats-wrapper">
                <Grid container spacing={3} alignItems="stretch">
                  {/* JOB DETAILS */}
                  <Grid size={{ md: 4, sm: 12, xs: 12 }}>
                    <Paper className="stats-section">
                      <Typography variant="h6" className="section-title">
                        Job Details
                      </Typography>
                      <Grid container spacing={2} alignItems="stretch">
                        <Grid size={{ xs: 12, sm: 6 }}>
                          <StatCard
                            icon={<CheckCircle />}
                            title="Completed"
                            value={jobDetails?.completed || 0}
                            gradientClass="gradient-blue"
                          />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6 }}>
                          <StatCard
                            icon={<Pending />}
                            title="Running"
                            value={jobDetails?.running || 0}
                            gradientClass="gradient-orange"
                          />
                        </Grid>
                      </Grid>
                    </Paper>
                  </Grid>

                  {/* QRs PRINTED */}
                  <Grid size={{ md: 4, sm: 12, xs: 12 }}>
                    <Paper className="stats-section">
                      <Typography variant="h6" className="section-title">
                        QRs Printed
                      </Typography>
                      <Grid container spacing={2} alignItems="stretch">
                        <Grid size={{ xs: 12, sm: 6 }}>
                          <StatCard
                            icon={<PrecisionManufacturing />}
                            title="Production Line"
                            value={qrPrinting?.productionLine || 0}
                            gradientClass="gradient-green"
                          />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6 }}>
                          <StatCard
                            icon={<Print />}
                            title="Pre-Print"
                            value={qrPrinting?.prePrint || 0}
                            gradientClass="gradient-cyan"
                          />
                        </Grid>
                      </Grid>
                    </Paper>
                  </Grid>

                  {/* PICK SLIP */}
                  <Grid size={{ md: 4, sm: 12, xs: 12 }}>
                    <Paper className="stats-section">
                      <Typography variant="h6" className="section-title">
                        Pick Slip
                      </Typography>
                      <Grid container spacing={2} alignItems="stretch">
                        <Grid size={{ xs: 12, sm: 6 }}>
                          <StatCard
                            icon={<ReceiptLong />}
                            title="Received (EBS)"
                            value={pickSlip?.receivedEBS || 0}
                            gradientClass="gradient-purple"
                          />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6 }}>
                          <StatCard
                            icon={<Warehouse />}
                            title="Processed (WH)"
                            value={pickSlip?.processedWH || 0}
                            gradientClass="gradient-red"
                          />
                        </Grid>
                      </Grid>
                    </Paper>
                  </Grid>
                </Grid>
              </Box>
              {/* Filters + Chart */}
              <Grid container spacing={3} mt={3} alignItems="stretch">
                <Grid size={{ md: 4, sm: 12, xs: 12 }}>
                  <Card
                    className="chart-card"
                    sx={{
                      height: "100%",
                      display: "flex",
                      flexDirection: "column",
                    }}
                  >
                    <CardContent>
                      <Box className="chart-header">
                        <Grid>
                          {/* 🔹 Chart Title */}
                          <Typography variant="h6" className="chart-title">
                            QC Status Overview
                          </Typography>
                        </Grid>
                      </Box>
                      {qcStatusBreakdown &&
                      qcStatusBreakdown.every((item) => item.value === 0) ? (
                        <Box
                          sx={{
                            height: "200px",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            color: "rgb(74, 74, 74)",
                            fontWeight: 600,

                            fontSize: "1rem",
                            borderRadius: "12px",
                            background: "rgba(240,242,245,0.6)",
                          }}
                        >
                          No data available
                        </Box>
                      ) : (
                        <HighchartsReact
                          highcharts={Highcharts}
                          options={gaugeOptions}
                        />
                      )}
                    </CardContent>
                  </Card>
                </Grid>
                <Grid size={{ md: 8, sm: 12, xs: 12 }}>
                  <Card
                    className="chart-card"
                    sx={{
                      height: "100%",
                      display: "flex",
                      flexDirection: "column",
                    }}
                  >
                    <CardContent>
                      <Box className="chart-header">
                        <Grid>
                          {/* 🔹 Chart Title */}
                          <Typography variant="h6" className="chart-title">
                            Dispatch vs Receiving
                          </Typography>
                        </Grid>

                        {/* 🔹 Dropdowns below title */}
                        <Box className="chart-select-group d-flex" gap={3}>
                          <Box display="flex" alignItems="center" gap={2}>
                            <Typography
                              variant="body2"
                              sx={{
                                fontWeight: 600,
                                color: "#2563eb", // blue
                                display: "flex",
                                alignItems: "center",
                                gap: "4px",
                              }}
                            >
                              Total Dispatch:
                              <span style={{ color: "#1e3a8a" }}>
                                {dispatchReceiving?.totalDispatch || 0}
                              </span>
                            </Typography>

                            <Typography
                              variant="body2"
                              sx={{
                                fontWeight: 600,
                                color: "#dc2626", // red
                                display: "flex",
                                alignItems: "center",
                                gap: "4px",
                              }}
                            >
                              Total Received:
                              <span style={{ color: "#7f1d1d" }}>
                                {dispatchReceiving?.totalReceived || 0}
                              </span>
                            </Typography>
                          </Box>
                          {/* FROM dropdown */}
                          <Box>
                            <FormControl size="small" sx={{ mr: 2 }}>
                              <InputLabel>From</InputLabel>
                              {units.length > 0 && (
                                <Select
                                  value={selectedUnit}
                                  label="From"
                                  disabled
                                  sx={{
                                    pr: "20px",
                                    fontWeight: 500,
                                    display: "flex",
                                    alignItems: "center",
                                  }}
                                >
                                  {units?.map((unit, index) => (
                                    <MenuItem key={index} value={unit.value}>
                                      {unit.label}
                                    </MenuItem>
                                  ))}
                                </Select>
                              )}
                            </FormControl>

                            {/* TO dropdown */}
                            <FormControl
                              size="small"
                              sx={{
                                mt: { xs: 1, md: 0 },
                              }}
                            >
                              <InputLabel>To</InputLabel>
                              {units.length > 0 && (
                                <Select
                                  value={to}
                                  label="To"
                                  onChange={(e) => setTo(e.target.value)}
                                  sx={{
                                    pr: "20px",
                                    fontWeight: 500,
                                    display: "flex",
                                    alignItems: "center",
                                  }}
                                >
                                  {units?.map((unit, index) => (
                                    <MenuItem
                                      key={index}
                                      value={unit.value}
                                      disabled={unit.value === selectedUnit}
                                    >
                                      {unit.label}
                                    </MenuItem>
                                  ))}
                                </Select>
                              )}
                            </FormControl>
                          </Box>
                        </Box>
                      </Box>
                      <>
                        {!dispatchReceiving ||
                        !dispatchReceiving.receivedData ||
                        dispatchReceiving.receivedData.length === 0 ||
                        dispatchReceiving.receivedData.every(
                          (item) =>
                            (item.dispatch || 0) === 0 &&
                            (item.receive || 0) === 0,
                        ) ? (
                          <Box
                            sx={{
                              height: "200px",
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              color: "rgb(74, 74, 74)",
                              fontWeight: 600,

                              fontSize: "1rem",
                              borderRadius: "12px",
                              background: "rgba(240,242,245,0.6)",
                            }}
                          >
                            No data available
                          </Box>
                        ) : (
                          <HighchartsReact
                            highcharts={Highcharts}
                            options={areaOptions(
                              getUnitLabel(selectedUnit),
                              getUnitLabel(to),
                            )}
                          />
                        )}
                      </>
                    </CardContent>
                  </Card>
                </Grid>
              </Grid>
            </>
          )}
        </Box>
      ) : (
        <Box
          sx={{
            height: "85vh",
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          Please Select Unit from Header to use Application
        </Box>
      )}
    </>
  );
};
