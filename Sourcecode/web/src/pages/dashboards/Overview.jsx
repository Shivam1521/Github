import React, { useEffect } from "react";
import ReactApexChart from "react-apexcharts";
import { useDispatch, useSelector } from "react-redux";
import { fetchDashboardOverview } from "../../store/dashboardSlice";
import "./Dashboard.scss";

const sparkOpts = (color) => ({
  chart: {
    type: "line",
    sparkline: { enabled: true },
  },
  stroke: { curve: "smooth", width: 2 },
  colors: [color],
  tooltip: { enabled: false },
  fill: {
    type: "gradient",
    gradient: {
      opacityFrom: 0.35,
      opacityTo: 0,
    },
  },
});

const KpiCard = ({
  label,
  value,
  delta,
  deltaUp,
  color,
  sparkSeries,
}) => (
  <div className="kpi-card">
    <div className="kpi-top">
      <div className={`kpi-delta ${deltaUp ? "up" : "down"}`}>
        {delta}
      </div>
    </div>

    <div className="kpi-value" style={{ color }}>
      {value}
    </div>

    <div className="kpi-label">
      {label}
    </div>

    <ReactApexChart
      options={sparkOpts(color)}
      series={sparkSeries}
      type="area"
      height={40}
    />
  </div>
);

export const Overview = ({
  filters,
  onFilterChange,
  filtersData
}) => {

const dispatch = useDispatch();

const { overviewData, overviewLoading } = useSelector(
(state) => state.dashboard
);

useEffect(() => {
dispatch(
fetchDashboardOverview({
fromDate: null,
toDate: null,
unitId: null,
locationType: null,
bu: null,
section: null,
brand: null,
category: null
})
);
}, [dispatch]);

const overview = overviewData || {};


// ---------- LIVE VALUE + STATIC CURVES ----------
const kpiData = [
{
label:"Total Production Qty",
value: overview?.ovQrPrinting?.productionQty || 0,
//delta:"+8.4%",
deltaUp:true,
color:"#3b82f6",
sparkSeries:[
{
data:[98,112,105,130,118,142,155]
}
]
},

{
label:"Total Rejected Qty",
value: overview?.ovQrPrinting?.rejectionQty || 0,
//delta:"-2.1%",
deltaUp:false,
color:"#ef4444",
sparkSeries:[
{
data:[22,18,20,14,16,12,11]
}
]
},

{
label:"Production Qty %",
value:`${overview?.ovQrPrinting?.productionPercent || 0}%`,
//delta:"+1.3%",
deltaUp:true,
color:"#10b981",
sparkSeries:[
{
data:[88,90,91,92,93,94,95]
}
]
},

{
label:"Rejection Qty %",
value:`${overview?.ovQrPrinting?.rejectionPercent || 0}%`,
// delta:"-0.6%",
deltaUp:false,
color:"#f59e0b",
sparkSeries:[
{
data:[8,7,6.5,6,5.5,5,4.38]
}
]
}
];


// ---------- DONUT LIVE ----------
const plantData = overview?.ovPlant_Percent || [];

const donutSeries =
plantData.length > 0
? plantData.map(x =>
x.productionPercentage > 0
? x.productionPercentage
: x.qty
)
: [100];

// const donutOptions = {
// labels:
// plantData.length > 0
// ? plantData.map(x => x.plantName)
// : ["No Data"]
// };
const generateColors = (count) => {
  return Array.from({ length: count }, (_, index) => {
    const hue = (index * 360) / count;
    return `hsl(${hue}, 65%, 55%)`;
  });
};

const donutOptions = {
  labels:
    plantData.length > 0
      ? plantData.map(x => x.plantName)
      : ["No Data"],

  colors: generateColors(plantData.length),

  chart: {
    events: {
      dataPointSelection: function (event, chartContext, config) {

        const clickedPlant =
          plantData?.[config.dataPointIndex];

        if (!clickedPlant) return;

        const matchedUnit = filtersData?.units?.find(
          x => x.unitName === clickedPlant.plantName
        );

        onFilterChange({
          ...filters,
          unitId: matchedUnit?.unitId || null,
          unitName: matchedUnit?.unitName || "All"
        });
      }
    }
  }
};

// ---------- MONTH CHART LIVE ----------
const monthSeries = [
{
name:"Production",
data:
overview?.ovMonth_Wiseproduction?.map(x=>x.qty) || []
}
];

const formatK = (val) => {
  if (val >= 1000) {
    return (val / 1000).toFixed(1) + "K";
  }
  return val;
};
// const monthOptions = {
// xaxis:{
// categories:
// overview?.ovMonth_Wiseproduction?.map(x=>x.month) || []
// }
// };

const monthOptions = {
  chart: {
    type: "bar",
    toolbar: { show: false },

    events: {
      dataPointSelection: function (event, chartContext, config) {
        const clickedData =
          overview?.ovMonth_Wiseproduction?.[config.dataPointIndex];

        if (!clickedData) return;

        const [month, year] = clickedData.month.split("-");

        const lastDate = new Date(year, month, 0).getDate();

        onFilterChange({
          ...filters,
          fromDate: `${year}-${month}-01`,
          toDate: `${year}-${month}-${lastDate}`,
        });
      },
    },
  },

  xaxis: {
    categories:
      overview?.ovMonth_Wiseproduction?.map((x) => x.month) || [],
  },

  yaxis: {
    labels: {
      formatter: function (val) {
        return formatK(val);
      },
    },
  },

  dataLabels: {
    enabled: true,
    formatter: function (val) {
      return formatK(val);
    },
  },

  tooltip: {
    y: {
      formatter: function (val) {
        return formatK(val);
      },
    },
  },
};
// ---------- CLASS CHART LIVE ----------
const classSeries = [
{
name:"Production",
data:
overview?.ovClass_wiseproduction?.map(x=>x.qty) || []
}
];

const classOptions = {
  chart: {
    type: "bar",
    toolbar: { show: false },

    // events: {
    //   dataPointSelection: function (event, chartContext, config) {

    //     const clickedData =
    //       overview?.ovClass_wiseproduction?.[config.dataPointIndex];

    //     if (!clickedData) return;

    //     onFilterChange({
    //       ...filters,
    //       category: clickedData.className || null
    //     });
    //   },
    // },
  },

  xaxis: {
    categories:
      overview?.ovClass_wiseproduction?.map(x => x.className) || []
  },

  yaxis: {
    labels: {
      formatter: function (val) {
        return formatK(val);
      },
    },
  },

  dataLabels: {
    enabled: true,
    formatter: function (val) {
      return formatK(val);
    },
  },

  tooltip: {
    y: {
      formatter: function (val) {
        return formatK(val);
      },
    },
  },
};


if(overviewLoading){
return <div>Loading...</div>;
}


return (
<div className="overview-root">

{/* KPI */}
<div className="kpi-row">
{kpiData.map((item)=>(
<KpiCard
key={item.label}
{...item}
/>
))}
</div>


{/* Row 1 */}
<div className="chart-grid two-col">

<div className="chart-card">
<h3>PLANT-WISE CONTRIBUTION</h3>
<p>Plant share %</p>

<ReactApexChart
options={donutOptions}
series={donutSeries}
type="donut"
height={280}
/>
</div>


<div className="chart-card">
<h3>MONTH ON MONTH PRODUCTION</h3>
<p>Monthly Production Trend</p>

<ReactApexChart
options={monthOptions}
series={monthSeries}
type="bar"
height={280}
/>
</div>

</div>


{/* Row 2 */}
<div className="chart-grid one-col">

<div className="chart-card">
<h3>CLASS WISE PRODUCTION</h3>
<p>Contribution by Class</p>

<ReactApexChart
options={classOptions}
series={classSeries}
type="bar"
height={280}
/>
</div>

</div>

</div>
);

};

export default Overview;