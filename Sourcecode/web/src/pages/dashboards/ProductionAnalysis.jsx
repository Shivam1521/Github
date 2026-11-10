import React from "react";
import ReactApexChart from "react-apexcharts";
import "./Dashboard.scss";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchProductionAnalysis } from "../../store/dashboardSlice";


// ── Palette ───────────────────────────────────────────────────────
const C = {
  teal: "#0d5071",
  tealMid: "#1e8ab4",
  cyan: "#06b6d4",
  indigo: "#6366f1",
  green: "#10b981",
  amber: "#f59e0b",
  orange: "#f97316",
  slate: "#475569",
  purple: "#7007b2",
};

const months = [
  "Jan",
  "Feb",
  "Mar",
  "Apr",
  "May",
  "Jun",
  "Jul",
  "Aug",
  "Sep",
  "Oct",
  "Nov",
  "Dec",
];

// ── Shared chart base ─────────────────────────────────────────────
const baseChart = {
  toolbar: { show: false },
  background: "transparent",
  fontFamily: "'DM Sans', sans-serif",
  animations: { enabled: true, easing: "easeinout", speed: 500 },
};
const baseGrid = {
  borderColor: "#f1f5f9",
  strokeDashArray: 4,
  xaxis: { lines: { show: false } },
  padding: { top: -30, right: 0, bottom: -10, left: 0 },
};
const baseXaxis = {
  axisBorder: { show: false },
  axisTicks: { show: false },
  labels: { style: { colors: "#64748b", fontSize: "10px", fontWeight: 600 } },
};
const baseYaxis = {
  labels: { style: { colors: "#94a3b8", fontSize: "10px" } },
};
const baseLegend = {
  offsetY: -30,
  position: "top",
  horizontalAlign: "right",
  fontSize: "10px",
  labels: { colors: "#475569" },
  markers: { radius: 3, width: 9, height: 9 },
};

const CHART_H = 195;

// ── 1. Total Production vs Actual Qty ─────────────────────────────
const totalProdSeries = [
  {
    name: "Target Qty",
    data: [
      16000, 15000, 17000, 15500, 16500, 17000, 15000, 16000, 17500, 16000,
      15500, 17000,
    ],
  },
  {
    name: "Actual Qty",
    data: [
      14200, 13500, 15800, 14000, 15000, 16200, 13800, 15200, 16500, 14800,
      14200, 15900,
    ],
  },
];
const totalProdOptions = {
  chart: { ...baseChart, type: "bar" },
  colors: [C.tealMid, C.amber],
  plotOptions: {
    bar: {
      columnWidth: "55%",
      borderRadius: 4,
      borderRadiusApplication: "end",
      grouped: true,
    },
  },
  dataLabels: { enabled: false },
  xaxis: { ...baseXaxis, categories: months },
  yaxis: {
    ...baseYaxis,
    labels: {
      ...baseYaxis.labels,
      formatter: (v) => `${(v / 1000).toFixed(0)}k`,
    },
  
  },
  grid: baseGrid,
  legend: baseLegend,
  tooltip: {
    theme: "light",
    y: { formatter: (v) => `${v.toLocaleString()} Units` },
  },
  fill: {
    type: ["gradient", "solid"],
    gradient: {
      shade: "light",
      type: "vertical",
      shadeIntensity: 0.3,
      gradientToColors: [C.cyan],
      stops: [0, 100],
    },
  },
};

// ── 2. BU Wise ────────────────────────────────────────────────────
const buSeries = [
  { name: "Target", data: [52000, 38000, 25000] },
  { name: "Achieved", data: [47800, 36200, 22400] },
];
const buOptions = {
  chart: { ...baseChart, type: "bar" },
  colors: [C.indigo, C.cyan],
  plotOptions: {
    bar: {
      horizontal: true,
      barHeight: "55%",
      borderRadius: 4,
      borderRadiusApplication: "end",
      grouped: true,
    },
  },
  dataLabels: {
    enabled: true,
    style: { fontSize: "9px", fontWeight: 700, colors: ["#fff"] },
    formatter: (v) => `${(v / 1000).toFixed(1)}k`,
  },
  xaxis: {
    ...baseXaxis,
    categories: ["WD", "SWG", "Others"],
    labels: {
      ...baseXaxis.labels,
      formatter: (v) => `${(v / 1000).toFixed(0)}k`,
    },
  },
  yaxis: {
    labels: { style: { colors: "#334155", fontSize: "11px", fontWeight: 700 } },
  },
  grid: {
    ...baseGrid,
    xaxis: { lines: { show: true } },
    yaxis: { lines: { show: false } },
  },
  legend: baseLegend,
  tooltip: {
    theme: "light",
    y: { formatter: (v) => `${v.toLocaleString()} Units` },
  },
};

// ── 3. Section Wise ───────────────────────────────────────────────
const sectionSeries = [
  { name: "Target", data: [42000, 33000, 28000] },
  { name: "Achieved", data: [39500, 30800, 25200] },
];
const sectionOptions = {
  chart: { ...baseChart, type: "bar" },
  colors: [C.teal, C.green],
  plotOptions: {
    bar: {
      horizontal: true,
      barHeight: "55%",
      borderRadius: 4,
      borderRadiusApplication: "end",
      grouped: true,
    },
  },
  dataLabels: {
    enabled: true,
    style: { fontSize: "9px", fontWeight: 700, colors: ["#fff"] },
    formatter: (v) => `${(v / 1000).toFixed(1)}k`,
  },
  xaxis: {
    ...baseXaxis,
    min: 0,
    tickAmount: 5,
    categories: ["Modular", "Non Modular", "Smart Anchor"],
    labels: {
      ...baseXaxis.labels,
      offsetX: 0,
      formatter: (v) => `${(v / 1000).toFixed(0)}k`,
    },
  },
  yaxis: {
    labels: {
      style: { colors: "#334155", fontSize: "10px", fontWeight: 700 },
      formatter: (val) => {
        if (val === "Non Modular") return ["Non", "Modular"];
        if (val === "Smart Anchor") return ["Smart", "Anchor"];
        return val;
      },
    },
  },
  grid: {
    ...baseGrid,
    padding: { top: -40, right: 0, bottom: -10, left: 8 },
    xaxis: { lines: { show: true } },
    yaxis: { lines: { show: false } },
  },
  legend: baseLegend,
  tooltip: {
    theme: "light",
    y: { formatter: (v) => `${v.toLocaleString()} Units` },
  },
};

// ── 4. Brand Wise ─────────────────────────────────────────────────
const brandSeries = [
  { name: "Target", data: [55000, 42000, 18000] },
  { name: "Achieved", data: [51200, 39600, 16400] },
];
const brandOptions = {
  chart: { ...baseChart, type: "bar" },
  colors: [C.tealMid, "#06c780"],
  plotOptions: {
    bar: {
      columnWidth: "48%",
      borderRadius: 5,
      borderRadiusApplication: "end",
      grouped: true,
    },
  },
  dataLabels: {
    enabled: true,
    offsetY: -7,
    style: { fontSize: "9px", fontWeight: 700, colors: [C.slate] },
    formatter: (v) => `${(v / 1000).toFixed(1)}k`,
  },
  xaxis: { ...baseXaxis, categories: ["Penta", "Viva", "Others"] },
  yaxis: {
    ...baseYaxis,
    labels: {
      ...baseYaxis.labels,
      formatter: (v) => `${(v / 1000).toFixed(0)}k`,
    },
  },
  grid: baseGrid,
  legend: baseLegend,
  tooltip: {
    theme: "light",
    y: { formatter: (v) => `${v.toLocaleString()} Units` },
  },
  fill: {
    type: ["gradient", "solid"],
    gradient: {
      shade: "light",
      type: "vertical",
      shadeIntensity: 0.25,
      gradientToColors: [C.cyan],
      stops: [0, 100],
    },
  },
};

// ── 5. Monthly Trend ─────────────────────────────────────────────
const trendSeries = [
  {
    name: "FY 2024-25",
    data: [
      14200, 13500, 15800, 14000, 15000, 16200, 13800, 15200, 16500, 14800,
      14200, 15900,
    ],
  },
];
const trendOptions = {
  chart: { ...baseChart, type: "line" },
  colors: [C.purple],
  stroke: { width: 3, curve: "smooth" },
  markers: { size: 4, strokeWidth: 2, strokeColors: "#fff" },
  fill: { type: "solid" },
  dataLabels: { enabled: false },
  xaxis: { ...baseXaxis, categories: months },
  yaxis: {
    ...baseYaxis,
    labels: {
      ...baseYaxis.labels,
      formatter: (v) => `${(v / 1000).toFixed(0)}k`,
      offsetX: -10,
    },
  },
  grid: { ...baseGrid, padding: { left: 10 } },
  legend: {
    show: true,
    offsetY: -30,
    position: "bottom",
    horizontalAlign: "right",
    fontSize: "10px",
    labels: { colors: "#475569" },
    markers: { radius: 3, width: 9, height: 9 },
  },
  tooltip: {
    theme: "light",
    shared: true,
    y: { formatter: (v) => `${v.toLocaleString()} Units` },
  },
};

// ── Achievement Badge ─────────────────────────────────────────────
const AchieveBadge = ({ pct, color }) => (
  <span className="achieve-badge" style={{ "--ab-color": color }}>
    <span className="achieve-dot" />
    {pct}% achieved
  </span>
);

// ── Chart Card ────────────────────────────────────────────────────
const ChartCard = ({ title, sub, badge, badgeColor, children }) => (
  <div className="chart-card">
    <div className="chart-header">
      <div>
        <div className="chart-title">{title}</div>
        {sub && <div className="chart-sub">{sub}</div>}
      </div>
      {badge && <AchieveBadge pct={badge} color={badgeColor || C.teal} />}
    </div>
    {children}
  </div>
);

// ── Production Analysis ───────────────────────────────────────────
export const ProductionAnalysis = () => {

 
  const dispatch = useDispatch();

  const { productionAnalysis, loading } = useSelector(
    (state) => state.dashboard
  );

// useEffect(() => {
//   dispatch(
//     fetchProductionAnalysis({
//       fromDate: null,
//       toDate: null,
//       unitId: null,
//       locationType: null,
//       supervisorid: null,
//       category: null
//     })
//   );
// }, [dispatch]);

  const rawData =
    productionAnalysis?.ovMonth_Wiseproduction || [];

const formatMonth = (m) => {
  const parts = m.split("-");

  const month =
    parts.length === 2
      ? parts[0] // "04-2025"
      : parts[1]; // "2025-04"

  return months[parseInt(month, 10) - 1] || "";
};

const trendCategories =
  rawData.length > 0
    ? rawData.map((item) => formatMonth(item.month))
    : months;

const trendSeriesDynamic =
  rawData.length > 0
    ? [
        {
          name: "Production Qty",
          data: rawData.map((item) => item.qty),
        },
      ]
    : trendSeries; // fallback static


if (loading) {
  return <div>Loading Production Analysis...</div>;
}
  // 🔥 AB RETURN START
  return (

  <div className="overview-root">
    {/* Row 1: BU Wise + Section Wise */}
    <div className="chart-grid two-col">
      <ChartCard
        title="Monthly Production Trend Qty"
        sub="Year-on-year comparison — Units produced per month"
        badge="95.6"
        badgeColor={C.teal}
      >
        <ReactApexChart
  options={{
    ...trendOptions,
    xaxis: {
      ...trendOptions.xaxis,
      categories: trendCategories,
    },
  }}
  series={trendSeriesDynamic}
  type="line"
  height={CHART_H}
/>
      </ChartCard>
      
      <ChartCard
        title="Production Achievement — BU Wise"
        sub="Target vs Achieved by Business Unit"
        badge="91.8"
        badgeColor={C.green}
      >
        <ReactApexChart
          options={buOptions}
          series={buSeries}
          type="bar"
          height={CHART_H}
        />
      </ChartCard>
     
    </div>

    {/* Row 2: Total Production — full width */}
    <div className="chart-grid one-col">
      <ChartCard
        title="Total Production vs Actual Qty"
        sub="Target vs Achieved — FY 2024-25 (Monthly)"
        badge="93.4"
        badgeColor={C.tealMid}
      >
        <ReactApexChart
          options={totalProdOptions}
          series={totalProdSeries}
          type="bar"
          height={CHART_H}
        />
      </ChartCard>
    </div>

    {/* Row 3: Brand Wise + Monthly Trend */}
    <div className="chart-grid two-col">
      <ChartCard
        title="Production Achievement — Brand Wise"
        sub="Target vs Achieved by Brand"
        badge="92.7"
        badgeColor={C.orange}
      >
        <ReactApexChart
          options={brandOptions}
          series={brandSeries}
          type="bar"
          height={CHART_H}
        />
      </ChartCard>
      

       <ChartCard
        title="Production Achievement — Section Wise"
        sub="Target vs Achieved by Section"
        badge="90.5"
        badgeColor={C.indigo}
      >
        <ReactApexChart
          options={sectionOptions}
          series={sectionSeries}
          type="bar"
          height={CHART_H}
        />
      </ChartCard>
 
      
    </div>
  </div>
  );
};
