import { useEffect, useState } from "react";
import { GamesClient, type GameDto } from "../../core/generated-client";
import { baseUrl } from "@core/baseUrl";
import Pagination from "../Pagination";
import { CircleSlash, SquarePen, Trash2 } from "lucide-react";
import { ActionMenu } from "@components/ActionMenu";
import { useTranslation } from "react-i18next";

export default function GamesList() {
  const { t } = useTranslation();
  const [games, setGames] = useState<GameDto[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  const pageSize = 10;

  const fetchGames = (page: number) => {
    const client = new GamesClient(baseUrl, {
      fetch: async (url, init) => {
        init = init ?? {};
        init.headers = {
          ...(init.headers ?? {}),
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        };
        return fetch(url, init);
      },
    });

    client
      .getAllGames(null, null, page, pageSize)
      .then((res) => {
        setGames(res.items);
        setTotalPages(Math.ceil(res.total / pageSize));
      })
      .catch(console.error);
  };

  useEffect(() => {
    fetchGames(currentPage);
  }, [currentPage]);

  const handleDelete = (game: GameDto) => {
    return undefined;
  };

  const handleEdit = (game: GameDto) => {
    return undefined;
  };
  if (games.length > 1) {
    return (
      <div>
        <p className="text-3xl mb-2">Games</p>

        <div className="relative overflow-visible bg-gray-800 rounded-2xl border border-gray-700 text-white ">
          <table className="w-full text-sm text-left rtl:text-right text-body">
            <thead className="{bg-gray-800/40}">
              <tr className={"px-2 bg-gray-700"}>
                <th
                  scope="col"
                  className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tl-xl"
                >
                  Week
                </th>
                <th
                  scope="col"
                  className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                >
                  Year
                </th>
                <th
                  scope="col"
                  className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                >
                  Start Time
                </th>
                <th
                  scope="col"
                  className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                >
                  Revenue
                </th>
                <th
                  scope="col"
                  className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                >
                  Winning Numbers
                </th>
                <th scope="col" className="py-3 rounded-tr-xl"></th>
              </tr>
            </thead>

            <tbody>
              {games.map((game, index) => {
                const isLast = index === games.length - 1;

                return (
                  <tr
                    key={game.id}
                    className={`border-b ${isLast ? "last:border-0" : ""}`}
                  >
                    <td
                      className={`max-w-[120px] px-3 py-2 text-ellipsis ${isLast ? "rounded-bl-xl" : ""}`}
                    >
                      {game.weekNumber}
                    </td>

                    <td className="px-3 py-2">{game.year}</td>

                    <td className="px-3 py-2 whitespace-nowrap">
                      {game.startTime
                        ? new Date(game.startTime).toLocaleString()
                        : "-"}
                    </td>

                    <td className="px-3 py-2">{game.revenue}</td>

                    <td className="px-3 py-2 max-w-[150px] overflow-hidden text-ellipsis whitespace-nowrap">
                      {game.winningNumbers ?? "-"}
                    </td>

                    <td
                      className={`w-12 px-3 py-2 ${isLast ? "rounded-br-xl" : ""}`}
                    >
                      <ActionMenu
                        actions={[
                          {
                            label: "Edit",
                            icon: <SquarePen />,
                            onClick: () => handleEdit(game),
                          },
                          { separator: true },
                          {
                            label: "Delete",
                            color: "#ff0000",
                            icon: <Trash2 color="#ff0000" />,
                            onClick: () => handleDelete(game),
                          },
                        ]}
                      />
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>

        {totalPages > 1 && (
          <div className="mt-4 flex justify-center">
            <Pagination
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={setCurrentPage}
            />
          </div>
        )}
      </div>
    );
  } else {
    return (
      <div className="flex flex-col justify-center items-center h-full text-3xl text-white">
        <CircleSlash size={50} />
        <span>{t("no_games_found")}</span>
      </div>
    );
  }
}
