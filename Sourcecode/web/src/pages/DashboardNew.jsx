import React, { useState, useEffect, useRef, useLayoutEffect } from "react";
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
import loader from "../assets/gifs/loader.gif";
import Quality from "../assets/gifs/Quality.gif";
import Factory from "../assets/gifs/Factory.gif";
import pending from "../assets/gifs/pending.gif";
import plan from "../assets/gifs/plan.gif";
import truck from "../assets/gifs/truck.gif";
import pickslip from "../assets/gifs/pickslip.gif";
import Funnel from "highcharts/modules/funnel";
import "apexcharts/dist/apexcharts.css";
import ApexCharts from "apexcharts";

// Initialize Highcharts modules
if (typeof HighchartsMore === "function") HighchartsMore(Highcharts);
if (typeof Highcharts3D === "function") Highcharts3D(Highcharts);
if (typeof Highcharts3D === "function") SolidGauge(Highcharts);
if (typeof Highcharts3D === "function") Accessibility(Highcharts);
if (typeof Highcharts3D === "function") Funnel(Highcharts);
export const DashboardNew = () => {
  const [selectedRange, setSelectedRange] = useState("week");
  const globalUnit = useSelector((state) => state.unit.selectedUnit);
  const [to, setTo] = useState(0);
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [selectedUnit, setSelectedUnit] = useState(globalUnit.toString());
  const { units } = useSelector((state) => state.lists);
  const { data, loading } = useSelector((state) => state.dashboard);
  const [filterType, setFilterType] = useState("date");
  const [selectedDate, setSelectedDate] = useState("");
  const [selectedYear, setSelectedYear] = useState("");
  const [selectedQuarter, setSelectedQuarter] = useState("");
  const [selectedBrand, setSelectedBrand] = useState("");
  const [selectedItem, setSelectedItem] = useState("");
  const funnelChartRef = useRef(null);

  const dispatch = useDispatch();
  const {
    // qcStatusBreakdown,
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
        toUnitId: to,
        fromDate,
        toDate,
      };
      dispatch(fetchDashboardData(newRequest));
    } else {
      navigate("/login");
    }
  }, [selectedUnit, to, fromDate, toDate]);

  const qcStatusBreakdown = [
    { label: "Complete", value: 80 },
    { label: "Hold", value: 40 },
    { label: "Accept", value: 90 },
    { label: "Reject", value: 25 },
  ];
  const total = qcStatusBreakdown.reduce((s, i) => s + i.value, 0);

  const STATUS_COLORS = {
    Complete: {
      linearGradient: { x1: 0, y1: 0, x2: 1, y2: 0 },
      stops: [
        [0, "#2563eb"], // blue
        [1, "#60a5fa"],
      ],
    },

    Hold: {
      linearGradient: { x1: 0, y1: 0, x2: 1, y2: 0 },
      stops: [
        [0, "#f97316"], // orange
        [1, "#fdba74"],
      ],
    },

    Accept: {
      linearGradient: { x1: 0, y1: 0, x2: 1, y2: 0 },
      stops: [
        [0, "#059669"], // green
        [1, "#34d399"],
      ],
    },

    Reject: {
      linearGradient: { x1: 0, y1: 0, x2: 1, y2: 0 },
      stops: [
        [0, "#dc2626"], // red
        [1, "#f87171"],
      ],
    },
  };

  const getPlannedData = (data) =>
    data.map((d) => ({
      name: d[0],
      y: d[1],
    }));

  const getActualData = (data) =>
    data.map((d) => ({
      name: d[0],
      y: d[2],
    }));
  const productionData = {
    current: [
      ["Jan", 1200, 1100],
      ["Feb", 1000, 950],
      ["Mar", 900, 980],
      ["Apr", 800, 760],
      ["May", 700, 720],
    ],
  };
  const PLAN_GRADIENT = {
    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
    stops: [
      [0, "rgba(203,213,225,0.9)"], // light slate
      [1, "rgba(148,163,184,0.9)"], // darker slate
    ],
  };
  const ACTUAL_GRADIENT = {
    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
    stops: [
      [0, "#38bdf8"], // sky blue
      [1, "#0284c7"], // deep blue
    ],
  };
  const pickSlipCategories = ["Jan", "Feb", "Mar", "Apr", "May"];

  const pendingData = [420, 380, 460, 510, 600];
  const processedData = [780, 860, 940, 1020, 1180];
  const PICKSLIP_COLORS = {
    processed: {
      linearGradient: { x1: 0, y1: 0, x2: 1, y2: 0 },
      stops: [
        [0, "#a855f7"], // soft violet
        [1, "#6b21a8"], // deep purple
      ],
    },

    pending: {
      linearGradient: { x1: 0, y1: 0, x2: 1, y2: 0 },
      stops: [
        [0, "#5eead4"], // soft teal
        [1, "#0f766e"], // deep teal
      ],
    },
  };

  const PICKSLIP_TOOLTIP_COLORS = {
    Pending: "#0f766e   ",
    Processed: "#6b21a8",
  };

  const pickSlipOptions = {
    chart: {
      type: "bar",
      height: 320,
      spacingTop: 0,
      spacingBottom: 0,
      backgroundColor: "transparent",
      style: {
        fontFamily: "Poppins, sans-serif",
      },
    },

    title: { text: null },

    subtitle: {
      text: "Pending v/s Processed",
      style: {
        color: "#64748b",
        fontSize: "13px",
        fontWeight: 500,
      },
    },

    xAxis: {
      categories: pickSlipCategories,
      lineWidth: 0,
      labels: {
        style: {
          fontSize: "12px",
          fontWeight: 600,
          color: "#334155",
        },
      },
    },

    yAxis: {
      min: 0,
      title: {
        text: "Pick Slips",
        style: {
          fontWeight: 600,
          color: "#475569",
        },
      },
      gridLineColor: "rgba(0,0,0,0.06)",
      labels: {
        style: {
          fontSize: "11px",
          fontWeight: 500,
          color: "#475569",
        },
      },
    },

    legend: {
      align: "center",
      verticalAlign: "bottom",
      symbolRadius: 6,
      itemStyle: {
        fontSize: "12px",
        fontWeight: 600,
        color: "#334155",
      },
    },

    tooltip: {
      shared: true,
      useHTML: true,
      backgroundColor: "rgba(255,255,255,0.95)",
      borderRadius: 10,
      shadow: false,

      formatter: function () {
        let html = `<b>${this.x}</b><br/>`;

        this.points.forEach((p) => {
          const color = PICKSLIP_TOOLTIP_COLORS[p.series.name] || "#334155";

          html += `
              <div style="
                display:flex;
                align-items:center;
                gap:6px;
                margin-top:4px;
              ">
                <span style="
                  width:8px;
                  height:8px;
                  border-radius:50%;
                  background:${color};
                  display:inline-block;
                "></span>
                <span style="color:#475569">${p.series.name}:</span>
                <b style="margin-left:auto">${p.y}</b>
              </div>
            `;
        });

        return html;
      },
    },

    plotOptions: {
      series: {
        stacking: "normal",

        // 🔥 BAR SIZE
        pointWidth: 26,
        groupPadding: 0.1,
        pointPadding: 0.03,

        borderRadius: 10,
        borderWidth: 0,

        shadow: {
          color: "rgba(0,0,0,0.15)",
          offsetX: 0,
          offsetY: 2,
          opacity: 0.6,
        },

        dataLabels: {
          enabled: true,
          inside: true,
          style: {
            fontSize: "11px",
            fontWeight: 600,
            color: "#fff",
            textOutline: "none",
          },
        },
      },
    },

    series: [
      {
        name: "Processed",
        data: processedData,
        color: PICKSLIP_COLORS.processed,
      },
      {
        name: "Pending",
        data: pendingData,
        color: PICKSLIP_COLORS.pending,
      },
    ],

    credits: { enabled: false },
    accessibility: { enabled: false },
  };

  const planOptions = {
    chart: {
      type: "column",
      animation: true,
      spacingTop: 10,
      spacingBottom: 20,
      height: 300,
      backgroundColor: "transparent",
    },

    title: { text: null },

    plotOptions: {
      series: {
        grouping: false,
        borderWidth: 0,
        borderRadius: 6,
        shadow: {
          color: "rgba(0,0,0,0.12)",
          offsetY: 2,
        },
      },
    },

    legend: {
      enabled: true,
      align: "center",
      verticalAlign: "bottom",
      itemStyle: {
        fontSize: "12px",
        fontWeight: 600,
        color: "#334155",
      },
      symbolRadius: 4,
    },

    tooltip: {
      shared: true,
      backgroundColor: "rgba(255,255,255,0.95)",
      borderRadius: 8,
      shadow: false,
      headerFormat: `<b>{point.key}</b><br/>`,
      pointFormat: `
        <span style="color:{series.color}">●</span>
        {series.name}: <b>{point.y}</b><br/>
      `,
    },

    xAxis: {
      type: "category",
      labels: {
        style: { fontWeight: 600, color: "#475569" },
      },
      lineColor: "#e5e7eb",
    },

    yAxis: {
      title: {
        text: "Production Quantity",
        style: { fontWeight: 600 },
      },
      labels: {
        style: { fontSize: "11px", color: "#475569" },
      },
      gridLineColor: "rgba(0,0,0,0.05)",
    },

    series: [
      {
        name: "Planned",
        pointPlacement: -0.2,
        color: PLAN_GRADIENT, // ✅ gradient
        data: getPlannedData(productionData.current),
      },
      {
        id: "actual",
        name: "Actual",
        color: ACTUAL_GRADIENT, // ✅ gradient
        dataSorting: {
          enabled: true,
          matchByName: true,
        },
        dataLabels: {
          enabled: true,
          inside: true,
          style: {
            fontSize: "13px",
            fontWeight: "700",
            color: "#ffffff",
            textOutline: "none",
          },
        },
        data: getActualData(productionData.current),
      },
    ],

    credits: { enabled: false },
  };

  const qcProgressOptions = {
    chart: {
      type: "bar",
      height: 300,
      backgroundColor: "transparent",
      spacingTop: -100,
      spacingBottom: 100,
      spacingLeft: 10,
      spacingRight: -10,
      style: { fontFamily: "Poppins, sans-serif" },
    },

    title: { text: null },

    xAxis: { visible: false },

    yAxis: {
      min: 0,
      max: total,
      visible: false,
    },

    tooltip: {
      formatter: function () {
        return `
            <span style="color:${this.color}">●</span>
            <b>${this.series.name}</b>: ${this.y}
          `;
      },
    },

    legend: {
      enabled: true,
      floating: true,
      align: "center",
      verticalAlign: "bottom",
      y: -10,
      layout: "vertical",

      navigation: {
        enabled: false,
      },

      symbolRadius: 5,
      symbolHeight: 10,
      symbolWidth: 10,
      itemMarginTop: 0,

      itemStyle: {
        fontSize: "13px",
        fontWeight: 600,
        color: "#334155",
      },

      labelFormatter: function () {
        const value = this.options?.data?.[0] ?? 0;
        return `
            <span style="display:flex;justify-content:space-between;width:130px";>
              <span style="font-size:12px;">${this.name}</span>
              <span style="font-weight:600;color:#64748b;font-size:12px;">${value}</span>
            </span>
          `;
      },

      useHTML: true,
    },
    plotOptions: {
      series: {
        stacking: "normal",
        pointWidth: 16,
        borderRadius: 10,
        groupPadding: 0,
        borderWidth: 2,
        borderColor: "#ffffff",
        pointPadding: 0,
        enableMouseTracking: true,
        shadow: {
          color: "rgba(0,0,0,0.25)",
          offsetX: 0,
          offsetY: 2,
          opacity: 0.6,
        },
      },
    },

    series: qcStatusBreakdown.map((item) => ({
      name: item.label,
      data: [item.value],
      color: STATUS_COLORS[item.label] || "#94a3b8",
    })),

    credits: { enabled: false },
    accessibility: { enabled: false },
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
  //   const categories = dispatchReceiving?.receivedData?.map((d) => d.label);
  //   const dispatchVals = dispatchReceiving?.receivedData?.map(
  //     (d) => d.dispatch ?? null
  //   );
  //   const receivedVals = dispatchReceiving?.receivedData?.map(
  //     (d) => d.receive ?? null
  //   );
  const categories = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

  const dispatchVals = [120, 180, 150, 220, 260, 200, 240];
  const receivedVals = [100, 160, 140, 200, 230, 190, 210];

  const DISPATCH_GRADIENT = {
    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
    stops: [
      [0, "#2dd4bf"], // teal
      [1, "#0f766e"], // deep teal
    ],
  };
  const RECEIVING_GRADIENT = {
    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
    stops: [
      [0, "#fbbf24"], // amber
      [1, "#d97706"], // orange
    ],
  };
  const TOOLTIP_COLORS = {
    Dispatch: "#0f766e", // deep teal
    Receiving: "#d97706", // amber
  };

  const barOptions = (from, to) => ({
    chart: {
      type: "column",
      backgroundColor: "transparent",
      height: 250,
      spacing: [20, 20, 0, 20],
      style: { fontFamily: "Poppins, sans-serif" },
    },

    title: { text: null },

    xAxis: {
      categories,
      lineColor: "#e5e7eb",
      tickColor: "#e5e7eb",
      labels: {
        style: {
          color: "#374151",
          fontSize: "12px",
          fontWeight: 500,
        },
      },
      title: {
        text: "Days",
        style: {
          color: "#475569",
          fontWeight: 600,
          fontSize: "12px",
        },
        margin: 12,
      },
    },

    yAxis: {
      min: 0,
      title: {
        text: "Quantity",
        style: {
          color: "#475569",
          fontWeight: 600,
          fontSize: "12px",
        },
      },
      labels: {
        style: {
          color: "#475569",
          fontSize: "11px",
          fontWeight: 500,
        },
      },
      gridLineColor: "rgba(0,0,0,0.05)",
    },

    tooltip: {
      shared: true,
      useHTML: true,
      backgroundColor: "rgba(255,255,255,0.95)",
      borderRadius: 10,
      shadow: false,

      formatter: function () {
        let html = `<b>${this.x}</b><br/>`;

        this.points.forEach((p) => {
          const name = p.series.name.includes("Dispatch")
            ? "Dispatch"
            : "Receiving";

          const color = TOOLTIP_COLORS[name] || "#334155";

          html += `
              <div style="display:flex;align-items:center;gap:6px">
                <span style="
                  width:8px;
                  height:8px;
                  border-radius:50%;
                  background:${color};
                  display:inline-block;
                "></span>
                <span style="color:#475569">${p.series.name}:</span>
                <b style="margin-left:auto">${p.y}</b>
              </div>
            `;
        });

        return html;
      },
    },

    legend: {
      align: "center",
      verticalAlign: "bottom",
      itemMarginTop: 8,
      itemStyle: {
        fontWeight: 500,
        color: "#334155",
        fontSize: "12px",
      },
    },

    plotOptions: {
      column: {
        borderRadius: 8,
        borderWidth: 0,
        pointPadding: 0.15,
        groupPadding: 0.12,
        shadow: {
          color: "rgba(0,0,0,0.12)",
          offsetX: 0,
          offsetY: 4,
          opacity: 0.5,
        },
      },
      series: {
        states: {
          hover: {
            brightness: 0.05,
          },
        },
      },
    },

    series: [
      {
        name: `Dispatch - ${from}`,
        data: dispatchVals,
        color: DISPATCH_GRADIENT,
      },
      {
        name: `Receiving - ${to}`,
        data: receivedVals,
        color: RECEIVING_GRADIENT,
      },
    ],

    credits: { enabled: false },
    accessibility: { enabled: false },
  });

  const categories2 = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul"];

  const completedJobs = [20, 35, 40, 55, 65, 70, 80];
  const pendingJobs = [60, 55, 50, 40, 30, 25, 15];

  const lineOptions = {
    chart: {
      type: "spline",
      spacingTop: 0,
      height: "320px",
      spacingLeft: 0,
      spacingBottom: 100,
      backgroundColor: "transparent",
      style: { fontFamily: "Poppins, sans-serif" },
    },

    title: { text: null },

    subtitle: {
      text: "Completed vs Pending Jobs",
      style: {
        color: "#64748b",
        fontSize: "13px",
      },
    },

    xAxis: {
      categories: categories2,
      title: {
        text: "Timeline",
        style: { fontWeight: 600 },
      },
      labels: {
        style: { fontSize: "12px", fontWeight: 500 },
      },
      lineWidth: 0,
    },

    yAxis: {
      min: 0,
      title: {
        text: "Number of Jobs",
        style: {
          fontWeight: 600,
          whiteSpace: "nowrap",
          textOverflow: "none",
        },
        rotation: -90,
        align: "middle",
        x: 0,
      },
      labels: {
        style: {
          fontSize: "12px",
          fontWeight: 500,
        },
      },
      gridLineColor: "rgba(0,0,0,0.05)",
    },

    legend: {
      align: "center",
      verticalAlign: "bottom",
      itemStyle: {
        fontWeight: 600,
        fontSize: "13px",
      },
    },

    tooltip: {
      shared: true,
      useHTML: true,
      backgroundColor: "rgba(255,255,255,0.95)",
      borderRadius: 8,
      shadow: false,
      formatter: function () {
        let html = `<b>${this.x}</b><br/>`;

        this.points.forEach((p) => {
          const seriesColor =
            typeof p.series.color === "object"
              ? p.series.color.stops[p.series.color.stops.length - 1][1]
              : p.series.color;

          html += `
              <div style="display:flex;align-items:center;gap:6px">
                <span style="
                  width:8px;
                  height:8px;
                  border-radius:50%;
                  background:${seriesColor};
                  display:inline-block;
                "></span>
                <span>${p.series.name}: <b>${p.y}</b></span>
              </div>
            `;
        });

        return html;
      },
    },

    plotOptions: {
      spline: {
        lineWidth: 4,
        marker: {
          radius: 5,
          symbol: "circle",
          lineWidth: 2,
          fillColor: "#ffffff",
        },
        states: {
          hover: {
            lineWidth: 5,
          },
        },
      },
    },

    series: [
      {
        name: "Completed Jobs",
        data: completedJobs,
        color: {
          linearGradient: { x1: 0, y1: 0, x2: 1, y2: 0 },
          stops: [
            [0, "#34d399"], // emerald
            [1, "#059669"], // deep green
          ],
        },
        marker: {
          lineColor: "#059669",
        },
        shadow: {
          color: "rgba(5,150,105,0.35)",
          width: 6,
        },
      },
      {
        name: "Pending Jobs",
        data: pendingJobs,
        color: {
          linearGradient: { x1: 0, y1: 0, x2: 1, y2: 0 },
          stops: [
            [0, "#fb7185"], // rose
            [1, "#dc2626"], // strong red
          ],
        },
        marker: {
          lineColor: "#dc2626",
        },
        shadow: {
          color: "rgba(220,38,38,0.35)",
          width: 6,
        },
      },
    ],

    credits: { enabled: false },
    accessibility: { enabled: false },
  };

  const funnelOptions = {
    chart: {
      type: "bar",
      height: 250,
      toolbar: { show: false },
      dropShadow: {
        enabled: true,
        top: 2,
        left: 0,
        blur: 6,
        opacity: 0.25,
      },
    },

    grid: {
      padding: {
        top: -20,
        bottom: 20,
        left: 0,
        right: 0,
      },
    },

    plotOptions: {
      bar: {
        horizontal: true,
        isFunnel: true,
        barHeight: "75%",
        borderRadius: 8,
        distributed: true,
      },
    },

    fill: {
      type: "gradient",
      gradient: {
        shade: "light",
        type: "horizontal",
        shadeIntensity: 0.6,
        opacityFrom: 1,
        opacityTo: 1,
        stops: [0, 100],
        colorStops: [
          [
            { offset: 0, color: "#ff6b6b", opacity: 1 },
            { offset: 100, color: "#c81e1e", opacity: 1 },
          ],
          [
            { offset: 0, color: "#ff9f43", opacity: 1 },
            { offset: 100, color: "#e8590c", opacity: 1 },
          ],
          [
            { offset: 0, color: "#ffd43b", opacity: 1 },
            { offset: 100, color: "#fab005", opacity: 1 },
          ],
          [
            { offset: 0, color: "#69db7c", opacity: 1 },
            { offset: 100, color: "#2f9e44", opacity: 1 },
          ],
          [
            { offset: 0, color: "#74c0fc", opacity: 1 },
            { offset: 100, color: "#1864ab", opacity: 1 },
          ],
        ],
      },
    },

    series: [
      {
        name: "Production",
        data: [30, 25, 20, 15, 10],
      },
    ],

    xaxis: {
      categories: ["P1", "P2", "P3", "P4", "P5"],
      labels: {
        style: {
          fontSize: "12px",
          fontWeight: 600,
          color: "#334155",
        },
      },
    },

    dataLabels: {
      enabled: true,
      formatter: (val, opts) => `${opts.w.globals.labels[opts.dataPointIndex]}`,
      style: {
        fontSize: "13px",
        fontWeight: 700,
        colors: ["#ffffff"],
      },
    },

    tooltip: {
      custom: function ({ series, seriesIndex, dataPointIndex, w }) {
        const label = w.globals.labels[dataPointIndex];
        const value = series[seriesIndex][dataPointIndex];
        const color =
          w.config.fill.gradient.colorStops[dataPointIndex][1].color;

        return `
          <div style="
            background:#fff;
            padding:8px 12px;
            border-radius:8px;
            box-shadow:0 6px 16px rgba(0,0,0,0.15);
            font-size:13px;
          ">
            <span style="
              display:inline-block;
              width:8px;
              height:8px;
              border-radius:50%;
              background:${color};
              margin-right:6px;
            "></span>
            <b>${label}</b>: ${value}
          </div>
        `;
      },
    },

    legend: { show: false },
  };

  useLayoutEffect(() => {
    if (!funnelChartRef.current) return;

    const chart = new ApexCharts(funnelChartRef.current, funnelOptions);

    chart.render();

    return () => {
      chart.destroy();
    };
  }, [qcStatusBreakdown]);
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
  const CURRENT_YEAR = new Date().getFullYear();
  const YEARS = Array.from(
    { length: CURRENT_YEAR - 2025 + 1 },
    (_, i) => CURRENT_YEAR - i
  );

  const QUARTERS = [
    { value: "Q1", label: "Q1" },
    { value: "Q2", label: "Q2" },
    { value: "Q3", label: "Q3" },
    { value: "Q4", label: "Q4" },
  ];
  const BRANDS = [
    { value: "PLASTIC BOXES", label: "PLASTIC BOXES" },
    { value: "ZIVA", label: "ZIVA" },
    { value: "WOODS", label: "WOODS" },
    { value: "EUROPA", label: "EUROPA" },
  ];
  const ITEMS = [
    {
      value: "10001 - UNO(6kA) 06A C SP MCB",
      label: "10001 - UNO(6kA) 06A C SP MCB",
    },
    {
      value: "10002 - UNO(6kA) 06A C SP MCB",
      label: "10002 - UNO(6kA) 10A C SP MCB",
    },
    {
      value: "10003 - UNO(6kA) 06A C SP MCB",
      label: "10003 - UNO(6kA) 10A C SP MCB",
    },
    {
      value: "10004 - UNO(6kA) 06A C SP MCB",
      label: "10004 - UNO(6kA) 10A C SP MCB",
    },
  ];

  const getQuarterRange = (year, quarter) => {
    switch (quarter) {
      case "Q1":
        return {
          from: `${year}-01-01`,
          to: `${year}-04-30`,
        };
      case "Q2":
        return {
          from: `${year}-05-01`,
          to: `${year}-08-31`,
        };
      case "Q3":
        return {
          from: `${year}-08-01`,
          to: `${year}-12-31`,
        };

      default:
        return { from: "", to: "" };
    }
  };
  useEffect(() => {
    // DATE filter
    if (filterType === "date" && selectedDate) {
      setFromDate(selectedDate);
      setToDate(selectedDate);
    }

    // YEAR filter
    if (filterType === "year" && selectedYear) {
      setFromDate(`${selectedYear}-01-01`);
      setToDate(`${selectedYear}-12-31`);
    }

    // QUARTER filter
    if (filterType === "quarter" && selectedYear && selectedQuarter) {
      const { from, to } = getQuarterRange(selectedYear, selectedQuarter);
      setFromDate(from);
      setToDate(to);
    }
  }, [filterType, selectedDate, selectedYear, selectedQuarter]);
  useEffect(() => {
    setSelectedDate("");
    setSelectedYear("");
    setSelectedQuarter("");
    setFromDate("");
    setToDate("");
  }, [filterType]);

  return (
    <Box className="dashboard-wrapper">
      {/* Header */}
      <Box className="dashboard-header new-style">
        <Box className="header-left">
          <Box>
            <Typography>Select Filter</Typography>

            {/* FILTER TYPE */}
            <FormControl size="small" className="unit-select">
              <Select
                value={filterType}
                onChange={(e) => setFilterType(e.target.value)}
                sx={{
                  pr: "20px",
                  fontWeight: 500,
                  display: "fle x",
                  height: "38px",
                  alignItems: "center",
                }}
              >
                <MenuItem value="date">Date</MenuItem>
                <MenuItem value="year">Year</MenuItem>
                <MenuItem value="quarter">Quarter</MenuItem>
              </Select>
            </FormControl>
          </Box>

          {/* DATE */}
          {filterType === "date" && (
            <FormControl size="small" className="unit-select">
              <TextField
                type="date"
                size="small"
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
                sx={{
                  "& input": {
                    fontWeight: 500,
                    cursor: "pointer",
                    fontSize: "14px",
                    height: "24px",
                  },
                }}
              />
            </FormControl>
          )}

          {/* YEAR */}
          {filterType === "year" && (
            <FormControl size="small" className="unit-select">
              <Select
                value={selectedYear}
                onChange={(e) => setSelectedYear(e.target.value)}
                displayEmpty
                renderValue={(year) =>
                  year ? (
                    <Box display="flex" alignItems="center" gap={1.5}>
                      <CalendarToday sx={{ fontSize: 18, color: "#475569" }} />
                      {year}
                    </Box>
                  ) : (
                    "Select Year"
                  )
                }
                sx={{
                  pr: "20px",
                  fontWeight: 500,
                  height: "38px",
                }}
              >
                <MenuItem value="">
                  <em>Select Year</em>
                </MenuItem>

                {YEARS.map((year) => (
                  <MenuItem key={year} value={year}>
                    {year}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}

          {/* QUARTER */}
          {filterType === "quarter" && (
            <Box sx={{ display: "flex", gap: 1 }}>
              <FormControl size="small" className="unit-select">
                <Select
                  value={selectedYear}
                  onChange={(e) => setSelectedYear(e.target.value)}
                  displayEmpty
                  renderValue={(year) =>
                    year ? (
                      <Box display="flex" alignItems="center" gap={1.5}>
                        <CalendarToday
                          sx={{ fontSize: 18, color: "#475569" }}
                        />
                        {year}
                      </Box>
                    ) : (
                      "Select Year"
                    )
                  }
                  sx={{
                    pr: "20px",
                    fontWeight: 500,
                    height: "38px",
                  }}
                >
                  <MenuItem value="">
                    <em>Select Year</em>
                  </MenuItem>

                  {YEARS.map((year) => (
                    <MenuItem key={year} value={year}>
                      {year}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
              <FormControl size="small" className="unit-select">
                <Select
                  value={selectedQuarter}
                  onChange={(e) => setSelectedQuarter(e.target.value)}
                  displayEmpty
                  renderValue={(quarter) =>
                    quarter ? (
                      <Box display="flex" alignItems="center" gap={1.5}>
                        <CalendarToday
                          sx={{ fontSize: 18, color: "#475569" }}
                        />
                        {quarter}
                      </Box>
                    ) : (
                      "Select Quarter"
                    )
                  }
                  sx={{
                    pr: "20px",
                    fontWeight: 500,
                    height: "38px",
                  }}
                >
                  <MenuItem value="">
                    <em>Select Quarter</em>
                  </MenuItem>
                  {QUARTERS.map((q) => (
                    <MenuItem key={q.value} value={q.value}>
                      {q.label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Box>
          )}
        </Box>
        <Box className="header-center">
          <FormControl size="small" className="unit-select" sx={{ mr: 1.5 }}>
            <Select
              value={selectedBrand}
              onChange={(e) => setSelectedBrand(e.target.value)}
              displayEmpty
              renderValue={(quarter) =>
                quarter ? (
                  <Box display="flex" alignItems="center" gap={1.5}>
                    {quarter}
                  </Box>
                ) : (
                  "Select Brand"
                )
              }
              sx={{
                pr: "20px",
                fontWeight: 500,
                height: "38px",
              }}
            >
              <MenuItem value="">
                <em>Select Brand</em>
              </MenuItem>
              {BRANDS.map((q) => (
                <MenuItem key={q.value} value={q.value}>
                  {q.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          {selectedBrand && (
            <FormControl size="small" className="unit-select" sx={{ mr: 1.5 }}>
              <Select
                value={selectedItem}
                onChange={(e) => setSelectedItem(e.target.value)}
                displayEmpty
                renderValue={(quarter) =>
                  quarter ? (
                    <Box display="flex" alignItems="center" gap={1.5}>
                      {quarter}
                    </Box>
                  ) : (
                    "Select Item"
                  )
                }
                sx={{
                  pr: "20px",
                  fontWeight: 500,
                  height: "38px",
                }}
              >
                <MenuItem value="">
                  <em>Select Item</em>
                </MenuItem>
                {ITEMS.map((q) => (
                  <MenuItem key={q.value} value={q.value}>
                    {q.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}
          {/* <Box className="date-wrapper">
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
          </Box> */}
        </Box>
      </Box>
      {/* {loading ? (
        <Backdrop open className="dashboard-loader">
          <Box
            component="img"
            src={loader}
            alt="Loading..."
            className="loader-image"
          />
        </Backdrop>
      ) : ( */}
      <>
        <Grid container spacing={1} mt={2} alignItems="stretch">
          <Grid size={{ md: 4, sm: 12, xs: 12 }}>
            <Card
              className="chart-card"
              sx={{
                height: "300px",
                display: "flex",
                flexDirection: "column",
              }}
            >
              <CardContent>
                <Box className="chart-header">
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "end",
                      justifyContent: "center",
                      gap: 1,
                    }}
                  >
                    <img src={Quality} alt="" width={30} height={30} />
                    <Typography variant="h6" className="chart-title">
                      QC Status Overview
                    </Typography>
                  </Box>
                  <Box
                    display="flex"
                    alignItems="center"
                    gap={2}
                    sx={{ mt: -1 }}
                  >
                    <Typography
                      variant="body2"
                      sx={{
                        fontWeight: 600,
                        color: "#0f766e", // deep teal
                        display: "flex",
                        alignItems: "center",
                        gap: "4px",
                      }}
                    >
                      Total Open:
                      <span style={{ color: "#134e4a", marginLeft: 4 }}>
                        {235}
                      </span>
                    </Typography>
                  </Box>
                </Box>
                {/* {qcStatusBreakdown &&
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
                  ) : ( */}
                <HighchartsReact
                  highcharts={Highcharts}
                  options={qcProgressOptions}
                />
                {/* )} */}
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ md: 4, sm: 12, xs: 12 }}>
            <Card
              className="chart-card"
              sx={{
                height: "300px",
                display: "flex",
                flexDirection: "column",
              }}
            >
              <CardContent>
                <Box className="chart-header">
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      gap: 1,
                      mb: -2,
                    }}
                  >
                    <img src={pending} alt="" width={40} height={40} />
                    <Typography variant="h6" className="chart-title">
                      Job Status
                    </Typography>
                  </Box>
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
                    options={lineOptions}
                  />
                )}
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ md: 4, sm: 12, xs: 12 }}>
            <Card
              className="chart-card"
              sx={{
                height: "300px",
                display: "flex",
                flexDirection: "column",
              }}
            >
              <CardContent>
                <Box className="chart-header">
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "flex-end",
                      justifyContent: "center",
                      mt: -1,
                    }}
                  >
                    <img src={Factory} alt="" width={60} height={40} />
                    <Typography variant="h6" className="chart-title">
                      Top 5 Productions
                    </Typography>
                  </Box>
                </Box>
                {/* {qcStatusBreakdown &&
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
                  ) : ( */}
                <div ref={funnelChartRef} />
                {/* )} */}
              </CardContent>
            </Card>
          </Grid>
        </Grid>
        <Grid container spacing={1} mt={1} alignItems="stretch">
          <Grid size={{ md: 12, sm: 12, xs: 12 }}>
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
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      gap: 1,
                    }}
                  >
                    <img src={pickslip} alt="" width={40} height={40} />
                    <Typography variant="h6" className="chart-title">
                      Pick Slip Status
                    </Typography>
                  </Box>
                </Box>
                {qcStatusBreakdown &&
                qcStatusBreakdown.every((item) => item.value === 0) ? (
                  <Box
                    sx={{
                      // height: "200px",
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
                    options={pickSlipOptions}
                  />
                )}
              </CardContent>
            </Card>
          </Grid>
        </Grid>
        <Grid container spacing={1} mt={1} alignItems="stretch">
          <Grid size={{ md: 6, sm: 12, xs: 12 }}>
            <Card
              className="chart-card"
              sx={{
                height: "400px",
                display: "flex",
                flexDirection: "column",
              }}
            >
              <CardContent>
                <Box className="chart-header">
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "start",
                      justifyContent: "center",
                      gap: 1,
                    }}
                  >
                    <img src={plan} alt="" width={50} height={50} />
                    <Typography variant="h6" className="chart-title">
                      Plan v/s Actual Production
                    </Typography>
                  </Box>
                </Box>
                {qcStatusBreakdown &&
                qcStatusBreakdown.every((item) => item.value === 0) ? (
                  <Box
                    sx={{
                      // height: "200px",
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
                    options={planOptions}
                  />
                )}
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ md: 6, sm: 12, xs: 12 }}>
            <Card
              className="chart-card"
              sx={{
                height: "400px",
                display: "flex",
                flexDirection: "column",
              }}
            >
              <CardContent>
                <Box className="chart-header">
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "start",
                      justifyContent: "center",
                      gap: 1,
                    }}
                  >
                    <img src={truck} alt="" width={50} height={50} />
                    <Typography variant="h6" className="chart-title">
                      Dispatch & Receiving
                    </Typography>
                  </Box>

                  {/* 🔹 Dropdowns below title */}
                  <Box className="chart-select-group d-flex" gap={3}>
                    {/* <Box
                        display="flex"
                        alignItems="center"
                        gap={2}
                        sx={{ mt: -1 }}
                      >
                        <Typography
                          variant="body2"
                          sx={{
                            fontWeight: 600,
                            color: "#0f766e", // deep teal
                            display: "flex",
                            alignItems: "center",
                            gap: "4px",
                          }}
                        >
                          Total Dispatch:
                          <span style={{ color: "#134e4a", marginLeft: 4 }}>
                            {dispatchReceiving?.totalDispatch || 0}
                          </span>
                        </Typography>

                        <Typography
                          variant="body2"
                          sx={{
                            fontWeight: 600,
                            color: "#d97706", // amber/orange
                            display: "flex",
                            alignItems: "center",
                            gap: "4px",
                          }}
                        >
                          Total Received:
                          <span style={{ color: "#92400e", marginLeft: 4 }}>
                            {dispatchReceiving?.totalReceived || 0}
                          </span>
                        </Typography>
                      </Box> */}

                    {/* FROM dropdown */}
                    <Box>
                      <FormControl
                        size="small"
                        sx={{ mr: 2, maxWidth: "170px" }}
                      >
                        <InputLabel>From</InputLabel>
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
                      </FormControl>

                      {/* TO dropdown */}
                      <FormControl size="small" sx={{ maxWidth: "170px" }}>
                        <InputLabel>To</InputLabel>
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
                      </FormControl>
                    </Box>
                  </Box>
                </Box>
                <>
                  {/* {!dispatchReceiving ||
                    !dispatchReceiving.receivedData ||
                    dispatchReceiving.receivedData.length === 0 ||
                    dispatchReceiving.receivedData.every(
                      (item) =>
                        (item.dispatch || 0) === 0 && (item.receive || 0) === 0
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
                    ) : ( */}
                  <HighchartsReact
                    highcharts={Highcharts}
                    options={barOptions(
                      getUnitLabel(selectedUnit),
                      getUnitLabel(to)
                    )}
                  />
                  {/* )} */}
                </>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </>
      {/* )} */}
    </Box>
  );
};
